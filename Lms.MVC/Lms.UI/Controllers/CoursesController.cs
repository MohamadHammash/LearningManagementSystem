using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.Data.Repositories.Helpers;
using Lms.MVC.UI.Filters;
using Lms.MVC.UI.Models.ViewModels.ApplicationUserViewModels;
using Lms.MVC.UI.Models.ViewModels.CourseViewModels;
using Lms.MVC.UI.Utilities.Pagination;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lms.MVC.UI.Controllers
{
    public class CoursesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IMapper mapper;

        private readonly IUoW uow;

        public CoursesController(IUoW uow, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            this.uow = uow;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public ActionResult ShowMyClassMates(int courseId, string id, string search, string sortOrder, int page)
        {  
            if (search != null)
            {
                page = 1;
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            id = uow.UserRepository.GetAllUsersAsync().Result.Where(u => u.Email == userEmail).FirstOrDefault().Id;

            courseId = uow.UserRepository.GetUserByIdAsync(id, true).Result.Courses.FirstOrDefault().Id;

            var coursesStudents = uow.CourseRepository.GetAllCoursesAsync(false, true).Result.FirstOrDefault(c => c.Id == courseId).Users.Where(u => u.Role == RoleHelper.Student);

            var result = mapper.Map<IEnumerable<ListApplicationUsersViewModel>>(coursesStudents);

            // Add to get the search to work
            if (!string.IsNullOrWhiteSpace(search))
            {
             result =  result.Where(u => u.Name.ToLower().StartsWith(search.ToLower()) || u.Email.ToLower().Contains(search.ToLower()));
            }
            ViewData["CurrentFilter"] = search;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "Email_desc" : "Email";

            result = sortOrder switch
            {
                "Name_desc" => result.OrderByDescending(s => s.Name),
                "Email" => result.OrderBy(s => s.Email),
                "Email_desc" => result.OrderByDescending(s => s.Email),
                _ => result.OrderBy(s => s.Name),
            };

            var paginatedResult = result.AsQueryable().GetPagination(page, 10);

            uow.CourseRepository.SetAllCoursesEndDate();

            return View(paginatedResult);
        }
        // GET: Courses
        [Authorize(Roles ="Teacher,Admin")]
        public async Task<IActionResult> Index(string search, string sortOrder, int page)
        {
            if (search != null)
            {
                page = 1;
            }

            string showOnlyMyCourses = Request.Cookies["ShowOnlyMyCourses"];
            IEnumerable<Course> courses;

            var currentUser = await userManager.GetUserAsync(User);

            if (showOnlyMyCourses == "true")
            {
                courses = uow.CourseRepository.GetAllCoursesAsync(false, true).Result
            .Where(c => (string.IsNullOrEmpty(search) || (c.Title.Contains(search))) && (c.Users != null && (c.Users.Contains(currentUser))));
            }
            else
            {
                courses = uow.CourseRepository.GetAllCoursesAsync(false, true).Result
                .Where(c => string.IsNullOrEmpty(search) || (c.Title.Contains(search)));
            }

            var result = mapper.Map<IEnumerable<ListCourseViewModel>>(courses);

            ViewData["CurrentFilter"] = search;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "StartDate" ? "StartDate_desc" : "StartDate";
            ViewData["EndDateSortParm"] = sortOrder == "EndDate" ? "EndDate_desc" : "EndDate";

            switch (sortOrder)
            {
                case "Title_desc":
                    result = result.OrderByDescending(s => s.Title);
                    break;

                case "StartDate":
                    result = result.OrderBy(s => s.StartDate);
                    break;

                case "StartDate_desc":
                    result = result.OrderByDescending(s => s.StartDate);
                    break;

                case "EndDate_desc":
                    result = result.OrderByDescending(s => s.EndDate);
                    break;

                case "EndDate":
                    result = result.OrderByDescending(s => s.EndDate);
                    break;

                default:
                    result = result.OrderBy(s => s.Title);
                    break;
            }

            var paginatedResult = result.AsQueryable().GetPagination(page, 10);
            uow.CourseRepository.SetAllCoursesEndDate();
            return View(paginatedResult);
        }

        public IActionResult ToggleMyCourses()
        {
            string showOnlyMyCourses = Request.Cookies["ShowOnlyMyCourses"];

            CookieOptions option = new();
            option.Expires = DateTime.Now.AddDays(900);

            if (showOnlyMyCourses == "true")
            {
                Response.Cookies.Append("ShowOnlyMyCourses", "false", option);
            }
            else
            {
                Response.Cookies.Append("ShowOnlyMyCourses", "true", option);
            }

            return RedirectToAction("Index", "Courses");
        }

        public async Task<IActionResult> RegisterForCourseToggle(int? id)
        {
            if (id == null) return NotFound();
            var course = await uow.CourseRepository.GetCourseAsync(id, false, true);
            var currentUser = await userManager.GetUserAsync(User);
            var teacher = userManager.Users.Include(x => x.Courses).Single(u => u == currentUser);

            if (course.Users.Contains(currentUser))
            {
                course.Users.Remove(currentUser);
                currentUser.Courses.Remove(course);
            }
            else
            {
                course.Users.Add(teacher);
                currentUser.Courses.Add(course);
            }

            await uow.CompleteAsync();
            return RedirectToAction("Index", "Courses");
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var course = await uow.CourseRepository.GetCourseAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            course = await uow.CourseRepository.SetCourseEndDateAsync((int)id);
            var model = mapper.Map<DetailCourseViewModel>(course);

            return View(model);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher,Admin")]
        public IActionResult Create()
        {
            var courseViewModel = new CreateCourseViewModel();
            courseViewModel.StartDate = DateTime.Now;
            return View(courseViewModel);
        }

        // POST: Courses/Create To protect from overposting attacks, enable the specific properties
        // you want to bind to. For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                courseViewModel.Users = new List<ApplicationUser>();
                if (User.IsInRole("Teacher"))
                {
                    courseViewModel.Users.Add(userManager.FindByIdAsync(userManager.GetUserId(User)).Result);
                }

                var course = mapper.Map<Course>(courseViewModel);

                course.EndDate = DateTime.Now.AddMonths(1);

                await uow.CourseRepository.AddAsync(course);
                await uow.CompleteAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        [ModelNotNull]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await uow.CourseRepository.GetCourseAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            // Create ViewModel
            var model = new EditCourseViewModel();
            mapper.Map(course, model);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        [ValidateAntiForgeryToken]
        [ModelValid, ModelNotNull]
        public async Task<IActionResult> Edit(int id, EditCourseViewModel courseModel)
        {
            var course = await uow.CourseRepository.GetCourseAsync(id);
            await uow.CourseRepository.CalculateEndDateAsync(course.Id);
            courseModel.Modules = course.Modules;
            mapper.Map(courseModel, course);
            try
            {
                uow.CourseRepository.Update(course);
                await uow.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id).Result)
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

        //[Authorize(Roles = "Teacher,Admin")]
        //public IActionResult AssignTeachers()
        //{
        //    return View();
        //}

        //[Authorize(Roles = "Teacher,Admin")]
        //public async Task<IActionResult> AssignTeachers(int id, Teacher teacher)
        //{
        //    Course course = await db.Courses.Include(c => c.Teachers).FirstOrDefaultAsync(c => c.Id == id);

        // if (course is null) { return NotFound(); }

        // if (course.Teachers is null) { course.Teachers = new List<Teacher>(); }

        // course.Teachers.Add(teacher);

        // db.Update(course); await db.SaveChangesAsync();

        //    return View();
        //}

        //[Authorize(Roles = "Teacher,Admin")]
        //public async Task<IActionResult> RemoveTeacher(int? id, string teacherId)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        // var course = await db.Courses .FirstOrDefaultAsync(m => m.Id == id); var teacher = await
        // db.Teachers.FirstOrDefaultAsync(t => t.Id == teacherId); if (course == null) { return
        // NotFound(); }

        //    return View(teacher);
        //}

        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Teacher,Admin")]
        //public async Task<IActionResult> RemoveTeacher(int? courseId, string teacherId)
        //{
        //    if (courseId is null || string.IsNullOrEmpty(teacherId))
        //    {
        //        return BadRequest();
        //    }

        // Course course = await db.Courses.Include(c => c.Teachers).FirstOrDefaultAsync(c => c.Id
        // == courseId);

        // if (course.Teachers is null) { return BadRequest(); } Teacher teacher =
        // course.Teachers.FirstOrDefault(t => t.Id == teacherId);

        // if (!course.Teachers.Remove(teacher)) return NotFound();

        // db.Update(course); db.SaveChanges();

        //    return View();
        //}

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await uow.CourseRepository.GetCourseAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await uow.CourseRepository.GetCourseAsync(id);
            uow.CourseRepository.Remove(course);
            await uow.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CourseExists(int id)
        {
            return await uow.CourseRepository.CourseExists(id);
        }


    }
}