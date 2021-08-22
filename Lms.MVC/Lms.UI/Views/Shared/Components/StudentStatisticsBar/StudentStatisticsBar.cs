using System.Collections.Generic;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lms.MVC.UI.Views.Shared.Components.StudentStatisticsBar
{
    public class StudentStatisticsBar

    {
        public IEnumerable<int> LateAssignments { get; set; }

        public int NextDueAssignment { get; set; }

        public int CurrentModule { get; set; }

        public int NextModule { get; set; }

        public IEnumerable<ApplicationUser> Teachers { get; set; }

        public string CMAType { get; set; }
    }

    public class StudentStatisticsBarViewComponent : ViewComponent
    {
        private readonly IUoW uoW;

        private readonly UserManager<ApplicationUser> userManager;

        public StudentStatisticsBarViewComponent(IUoW uoW, UserManager<ApplicationUser> userManager)
        {
            this.uoW = uoW;
            this.userManager = userManager;
        }

        public IViewComponentResult Invoke(string cmaType, int? courseId, int? moduleId, string userId)
        {
            var bar = new StudentStatisticsBar();

            switch (cmaType.ToLower())
            {
                case "module":
                    bar.LateAssignments = uoW.ActivityRepository.GetAllLateAssignmentsFromModuleAsync((int)moduleId, userId).Result;
                    bar.CurrentModule = uoW.ModuleRepository.GetCurrentModule(courseId, moduleId);
                    bar.NextModule = uoW.ModuleRepository.GetNextModule(courseId, moduleId);
                    bar.NextDueAssignment = uoW.ActivityRepository.GetNextDueAssignment(courseId, moduleId);
                    bar.Teachers = uoW.CourseRepository.GetTeachersByModule(moduleId);

                    break;

                case "course":
                    bar.LateAssignments = uoW.ActivityRepository.GetAllLateAssignmentsFromCourseAsync((int)courseId, userId).Result;
                    bar.CurrentModule = uoW.ModuleRepository.GetCurrentModule(courseId, moduleId);
                    bar.NextModule = uoW.ModuleRepository.GetNextModule(courseId, moduleId);
                    bar.NextDueAssignment = uoW.ActivityRepository.GetNextDueAssignment(courseId, moduleId);
                    bar.Teachers = uoW.CourseRepository.GetTeachers(courseId);
                    break;

                default:
                    ModelState.AddModelError("", "Something went wrong");
                    break;
            }
            bar.CMAType = cmaType;

            return View(bar);
        }
    }
}