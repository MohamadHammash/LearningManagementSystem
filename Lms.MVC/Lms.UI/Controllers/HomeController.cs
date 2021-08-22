using System.Collections.Generic;
using System.Linq;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Models;
using Lms.MVC.UI.Models.ViewModels.Admin;

using Microsoft.AspNetCore.Mvc;

namespace Lms.MVC.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUoW uow;

        public HomeController(IUoW uow)
        {
            this.uow = uow;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Teacher"))
            {
                return RedirectToAction("Index", "Courses");
            }
            else if (User.IsInRole("Admin"))
            {
                var adminOverviewViewModel = new AdminOverviewViewModel();

                // Todo: refactor this and add getalladmins etc to the repository                
                adminOverviewViewModel.NumberOfCourses = GetNumberOfCourses();
                adminOverviewViewModel.NumberOfModules = GetNumberOfModules();
                adminOverviewViewModel.NumberOfActivities = GetNumberOfActivities();
                adminOverviewViewModel.NumberOfAdmins = GetNumberOfUsersWithTheRole( "Admin");
                adminOverviewViewModel.NumberOfStudents = GetNumberOfUsersWithTheRole( "Student");
                adminOverviewViewModel.NumberOfTeachers = GetNumberOfUsersWithTheRole( "Teacher");
                adminOverviewViewModel.NumberOfUsers = GetNumberOfUsers();

                return View("~/Views/AdminLanding/AdminOverview.cshtml", adminOverviewViewModel);
            }
            else if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Modules");
            }
            return null;
        }

        private int GetNumberOfUsers()
        {
            return uow.UserRepository.GetAllUsersAsync().Result.Count();
        }

        private int GetNumberOfUsersWithTheRole(string role)
        {
            return uow.UserRepository.GetAllUsersAsync().Result.Where(u => u.Role == role).Count();
        }

        private int GetNumberOfActivities()
        {
            return uow.ActivityRepository.GetAllActivitiesAsync().Result.Count();
        }

        private int GetNumberOfModules()
        {
            return uow.ModuleRepository.GetAllModulesAsync(false).Result.Count();
        }

        private int GetNumberOfCourses()
        {
            return uow.CourseRepository.GetAllCoursesAsync(false).Result.Count();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {            
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}