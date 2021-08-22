using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Filters;
using Lms.MVC.UI.Models.ViewModels.ApplicationUserViewModels;
using Lms.MVC.UI.Utilities.Pagination;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lms.MVC.UI.Controllers
{
    [Authorize(Roles = "Teacher,Admin,Student")]
    public class ApplicationUsersController : Controller
    {
        private readonly ILogger<ApplicationUsersController> _logger;

        private readonly IUoW uoW;

        private readonly IMapper mapper;

        public ApplicationUsersController(

            ILogger<ApplicationUsersController> logger,
            IEmailSender emailSender, IUoW uoW, IMapper mapper)
        {
            _logger = logger;

            this.uoW = uoW;
            this.mapper = mapper;
        }

        // GET: ApplicationUsersController
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Index(string search, string sortOrder, int page)
        {
            if (search != null)
            {
                page = 1;
            }
            var users = await uoW.UserRepository.GetAllUsersAsync();
            var userListWithoutSuperAdmin = users.Where(u => u.NormalizedEmail != "ADMIN@LMS.SE");

            var model = mapper.Map<IEnumerable<ListApplicationUsersViewModel>>(userListWithoutSuperAdmin);

            // Add to get the search to work
            if (!string.IsNullOrWhiteSpace(search))
            {
                model = model.Where(u => u.Name.ToLower().StartsWith(search.ToLower()) || u.Email.ToLower().Contains(search.ToLower()) || u.Role.ToLower().Trim() == search.ToLower());
            }

            ViewData["CurrentFilter"] = search;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewData["RoleSortParm"] = sortOrder == "Email" ? "Role_desc" : "Role";

            model = sortOrder switch
            {
                "Name_desc" => model.OrderByDescending(s => s.Name),
                "Email" => model.OrderBy(s => s.Email),
                "Email_desc" => model.OrderByDescending(s => s.Email),
                "Role" => model.OrderBy(s => s.Role),
                "Role_desc" => model.OrderByDescending(s => s.Role),
                _ => model.OrderBy(s => s.Name),
            };
            var paginatedResult = model.AsQueryable().GetPagination(page, 10);

            return View(paginatedResult);
        }

        // GET: ApplicationUsersController/Details/5
        [Authorize(Roles = "Teacher, Admin, Student")]
        [ModelNotNull]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await uoW.UserRepository.GetUserByIdAsync(id, true);

            var model = mapper.Map<DetailsApplicationUserViewModel>(user);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [ModelNotNull]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await uoW.UserRepository.GetUserByIdAsync(id, true);

            var model = mapper.Map<EditApplicationUserViewModel>(user);

            if (user == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(string id, EditApplicationUserViewModel viewmodel)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await uoW.UserRepository.GetUserByIdAsync(id, true);
            viewmodel.Courses = user.Courses;
            if (user.Email.ToLower().Contains("admin@lms.se"))
            {
                ModelState.AddModelError("Email", "You can't edit the admin");
            }

            if (await TryUpdateModelAsync(viewmodel, "", u => u.Name,
                                                    u => u.Email,
                                                    u => u.Role,
                                                    user => user.PhoneNumber,
                                                    user => user.Courses))
                if (viewmodel.Role is null)
                {
                    ModelState.AddModelError("Role", "Choose a role");
                }
            if (viewmodel.Role != "Admin" && viewmodel.Role != "Teacher" && viewmodel.Role != "Student")
            {
                ModelState.AddModelError("Role", "Choose a valid role");
            }
            if (ModelState.IsValid)
            {
                mapper.Map(viewmodel, user);

                await uoW.UserRepository.ChangeRoleAsync(user);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await uoW.CompleteAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(viewmodel);
        }

        private bool UserExists(string id)
        {
            return uoW.UserRepository.Any(id);
        }

        [Authorize(Roles = "Admin")]
        [ModelNotNull]
        public async Task<IActionResult> Remove(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userToBeRemoved = await uoW.UserRepository.GetUserByIdAsync(id, true);

            var model = mapper.Map<DeleteApplicationUserViewModel>(userToBeRemoved);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        [ModelNotNull]
        public async Task<IActionResult> RemoveConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userToBeRemoved = await uoW.UserRepository.GetUserByIdAsync(id, true);

            uoW.UserRepository.Remove(userToBeRemoved);
            await uoW.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Teacher,Admin")]
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> EmailExistsEdit(string email, string id)
        {
            var users = await uoW.UserRepository.GetAllUsersAsync();
            if (users.Any(u => email == u.Email && id != u.Id))
            {
                return Json($"Email already exits");
            }
            return Json(true);
        }

        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> EmailExistsCreate([Bind(Prefix = "Input.Email")] string email)
        {
            var users = await uoW.UserRepository.GetAllUsersAsync();
            if (users.Any(u => email == u.Email))
            {
                return Json($"Email already exits");
            }
            return Json(true);
        }
    }
}