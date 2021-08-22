using System;
using System.Collections.Generic;
using System.Linq;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;

using Microsoft.AspNetCore.Mvc;

// The component is implemented as below
// Valid CMATypes are user, course, module and activity
// The id is ALWAYS a string - it will be parsed in the component if the CMAType is a course, module or activity
//
//@using Microsoft.AspNetCore.Identity
//@inject UserManager<ApplicationUser> userManager
//
// You put the line below where you want the component to go - the bool indicates if the intended view is a teacher.
//
//@await Component.InvokeAsync("ListFiles", new { CMAType = "course", id = Model.CourseId.ToString() , userIsTeacher=true})

namespace Lms.MVC.UI.Views.Shared.Components.ListFiles
{
    public class ListFiles
    {
        public ICollection<ApplicationFile> FileList { get; set; }

        public string CMAType { get; set; }

        public string userId { get; set; }
    }

    public class ListFilesViewComponent : ViewComponent
    {
        public IUoW uow { get; set; }

        public ListFilesViewComponent(IUoW uow)
        {
            this.uow = uow;
        }

        public IViewComponentResult Invoke(string CMAType, string id, bool userIsTeacher, string userId)
        {
            ListFiles files = new ListFiles();
                files.userId = userId;
            if (CMAType.ToLower() == "user")
            {
                files.FileList = uow.UserRepository.GetAllFilesByUserId(id).Result;
            }
            if (CMAType.ToLower() == "course")
            {
                if (userIsTeacher)
                {
                    files.FileList = uow.CourseRepository.GetAllFilesByCourseId(Int32.Parse(id)).Result;
                }
                else
                {
                    files.FileList = uow.CourseRepository.GetAllFilesByCourseId(Int32.Parse(id)).Result.Where(f => f.Assignment == false).ToList();
                }
            }
            if (CMAType.ToLower() == "module")
            {
                if (userIsTeacher)
                {
                    files.FileList = uow.ModuleRepository.GetAllFilesByModuleId(Int32.Parse(id)).Result;
                }
                else
                {
                files.FileList = uow.ModuleRepository.GetAllFilesByModuleId(Int32.Parse(id)).Result.Where(f => f.Assignment == false).ToList();

                }
            }
            if (CMAType.ToLower() == "activity")
            {
                if (userIsTeacher)
                {
                    files.FileList = uow.ActivityRepository.GetAllFilesByActivityId(Int32.Parse(id)).Result;
                }
                else
                {
                    var activityFiles = uow.ActivityRepository.GetAllFilesByActivityId(Int32.Parse(id)).Result.Where(f => f.Assignment == false).ToList();
                    var userFiles = uow.UserRepository.GetAllFilesByUserId(userId).Result;
                    activityFiles.AddRange(userFiles);
                    files.FileList = activityFiles.Distinct().ToList();
                }
            }
            files.CMAType = CMAType.ToLower();

            return View(files);
        }
    }
}