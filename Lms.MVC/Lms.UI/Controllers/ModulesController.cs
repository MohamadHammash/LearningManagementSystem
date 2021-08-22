using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Itenso.TimePeriod;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.Data.Repositories.Helpers;
using Lms.MVC.UI.Filters;
using Lms.MVC.UI.Models.ViewModels.ApplicationUserViewModels;
using Lms.MVC.UI.Models.ViewModels.ModuleViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lms.MVC.UI.Controllers
{
    public class ModulesController : Controller
    {
        private readonly IMapper mapper;

        private readonly IUoW uow;

        private readonly UserManager<ApplicationUser> userManager;

        public ModulesController(IMapper mapper, IUoW uow, UserManager<ApplicationUser> userManager)
        {
            this.mapper = mapper;
            this.uow = uow;
            this.userManager = userManager;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index(int? Id)
        {
            // Todo: Should we make this a part of the entity?

            //Student View of Modules for Course
            if (User.IsInRole("Student"))
            {
                var user = await userManager.GetUserAsync(User);

                var userCourse = uow.CourseRepository.GetAllCoursesAsync(false, true).Result
                    .Where(c => c.Users.Any(u => u.Id == user.Id)).FirstOrDefault();

                var modules = uow.ModuleRepository.GetAllModulesAsync(false).Result
                    .Where(m => m.CourseId == userCourse.Id);

                var moduleViewModel = new ListModuleViewModel();
                moduleViewModel.ModuleList = modules.ToList();

                moduleViewModel.CourseId = userCourse.Id;
                moduleViewModel.CourseTitle = userCourse.Title;

                return View(moduleViewModel);
            }
            else
            {
                if (Id != null)
                {
                    var courseTitle = uow.CourseRepository.GetAllCoursesAsync(false).Result.Where(c => c.Id == Id).FirstOrDefault().Title;
                    var moduleViewModel = new ListModuleViewModel();
                    moduleViewModel.ModuleList = uow.ModuleRepository.GetAllModulesAsync(false).Result.Where(m => m.CourseId == Id).ToList();

                    moduleViewModel.CourseId = (int)Id;
                    moduleViewModel.CourseTitle = courseTitle;

                    return View(moduleViewModel);
                }
                return RedirectToAction("Index");//, "Courses");
            }

            // TODO: Everyone execept students go to modules and activities via course list no??

            //if (User.IsInRole("Teacher"))
            //{
            //    var user = GetUserByName();
            //    var courses = db.Courses.Where(c => c.Id == courseId).ToList();
            //    var modules = new List<Module>();

            //    foreach(var course in courses)
            //    {
            //        var modulesInCourse = db.Modules.Where(m => m.CourseId == course.Id).ToList();
            //        modules.AddRange(modulesInCourse);
            //    }
            //    return View(modules);
            //}
            //if (User.IsInRole("Admin"))
            //{
            //    return View(await db.Modules.ToListAsync());
            //}
            //else return View();
        }

        private ApplicationUser GetUserByName()
        {
            return uow.UserRepository.GetAllUsersAsync().Result.FirstOrDefault(u => u.Name == User.Identity.Name);
        }

        // This is an unneccecary method.. the link should redirecto to activities
        [HttpGet]

        //[Route("details/{title}")]//Todo Fix Navigation
        public ActionResult Details(int id, string title)
        {
            //Find course
            var course = uow.CourseRepository.GetCourseAsync(id).Result;
            course.Modules = GetModules(course.Id);

            //Find module in course
            var module = course.Modules.FirstOrDefault(m => m.Title == title);

            //Add Activities
            module.Activities = GetActivities(module.Id);

            //mapper.Map<ModuleDto>(module);

            return View(module);
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpGet]
        [Route("new")]
        public ActionResult Create(int Id)
        {
            var moduleViewModel = new CreateModuleViewModel();
            moduleViewModel.CourseId = Id;
            if (uow.CourseRepository.GetCourseAsync(Id, true).Result.Modules.Count > 0)
            {
                var modules = uow.CourseRepository.GetCourseAsync(Id, true).Result.Modules;
                moduleViewModel.StartDate = modules.Last().EndDate;
            }
            else
            {
                moduleViewModel.StartDate = uow.CourseRepository.GetCourseAsync(Id).Result.StartDate.AddSeconds(1);
            }
            moduleViewModel.EndDate = moduleViewModel.StartDate.AddDays(1);
            return View(moduleViewModel);
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ModelValid, Route("new")]
        public async Task<IActionResult> Create(CreateModuleViewModel moduleViewModel)//TODO: Configure API
        {
            //Find Module
            var courses = await uow.CourseRepository.GetAllCoursesAsync(true);
            var currentCourse = courses.Where(c => c.Id == moduleViewModel.CourseId).FirstOrDefault();

            var modules = uow.ModuleRepository.GetAllModulesAsync(false).Result;
            var modulesInCurrentCourse = modules.Where(a => a.CourseId == currentCourse.Id);

            ValidateDates(moduleViewModel, currentCourse, modulesInCurrentCourse);

            if (ModelState.IsValid)
            {
                // Map view model to model
                var module = mapper.Map<Module>(moduleViewModel);

                //Add Module to Course
                currentCourse.Modules.Add(module);

                // Update course end date
                currentCourse = await uow.CourseRepository.SetCourseEndDateAsync(currentCourse.Id);

                await uow.CompleteAsync();

                // Send user back to list of modules for that course
                return RedirectToAction("Index", "Modules", new { id = moduleViewModel.CourseId });
            }
            return View(moduleViewModel);
        }

        private void ValidateDates(CreateModuleViewModel moduleViewModel, Course currentCourse, IEnumerable<Module> modulesInCurrentCourse)
        {
            TimePeriodCollection activitiesTimeperiod = new();
            TimeRange activityTimeRange = new TimeRange(moduleViewModel.StartDate, moduleViewModel.EndDate);

            if (modulesInCurrentCourse.Count() > 0)
            {
                foreach (var item in modulesInCurrentCourse)
                {
                    activitiesTimeperiod.Add(new TimeRange(item.StartDate, item.EndDate));
                }
                if (activitiesTimeperiod.IntersectsWith(activityTimeRange))
                {
                    ModelState.AddModelError("", $"Dates overlap other modules in this course");
                }
            }

            if (moduleViewModel.StartDate < currentCourse.StartDate)
            {
                ModelState.AddModelError("StartDate", "Modules start date is before Course start date");
            }
            if (moduleViewModel.StartDate > moduleViewModel.EndDate)
            {
                ModelState.AddModelError("EndDate", "A module cannot end before it starts");
            }
        }

        [HttpGet]
        [Route("edit/{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        public ActionResult Edit(int Id)
        {
            //find and create display details of Module
            var module = uow.ModuleRepository.GetAllModulesAsync(false).Result.FirstOrDefault(m => m.Id == Id);
            EditModuleViewModel model = new EditModuleViewModel()
            {
                Id = module.Id,
                Title = module.Title,
                Description = module.Description,
                StartDate = module.StartDate,
                EndDate = module.EndDate
            };
            return View(model);
        }

        [HttpPost]
        [Route("edit/{id}")]
        [Authorize(Roles = "Teacher, Admin")]
        [ValidateAntiForgeryToken]
        [ModelValid]
        public async Task<ActionResult> Edit(int id, EditModuleViewModel moduleViewModel)
        {
            //find module
            var module = uow.ModuleRepository.GetModuleAsync(id).Result;

            try
            {
                //mapper.Map(moduleDto, module);
                module.Title = moduleViewModel.Title;
                module.Description = moduleViewModel.Description;
                module.StartDate = moduleViewModel.StartDate;
                module.EndDate = moduleViewModel.EndDate;

                uow.ModuleRepository.Update(module);
                await uow.CompleteAsync();

                return RedirectToAction("Index", "Activities",new { Id = id });
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        [Route("delete")]
        [Authorize(Roles = "Teacher, Admin")]
        public ActionResult Delete(int id)
        {
            var module = uow.ModuleRepository.GetModuleAsync(id);
            if (module == null)
                return NotFound();

            var model = mapper.Map<ListModuleViewModel>(module);
            if (model == null) return View();

            return View(model);
        }

        [Route("delete")]
        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ListModuleViewModel module)
        {
            try
            {
                var moduleDb = uow.ModuleRepository.GetModuleAsync(id).Result;

                if (moduleDb == null || moduleDb.Id != module.Id)
                    return View();

                uow.ModuleRepository.Remove(moduleDb);
                uow.CompleteAsync();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private List<Module> GetModules(int id)
        {
            var modules = new List<Module>();

            foreach (var module in uow.ModuleRepository.GetAllModulesAsync(false).Result)
            {
                if (module.CourseId == id)
                {
                    modules.Add(module);
                }
            }
            return modules;
        }

        private List<Activity> GetActivities(int id)
        {
            var activities = new List<Activity>();

            foreach (var activity in uow.ActivityRepository.GetAllActivitiesAsync().Result)
            {
                if (activity.ModuleId == id)
                {
                    activities.Add(activity);
                }
            }

                return activities;
            }        

    }
}