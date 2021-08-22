using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lms.MVC.Core.Repositories;

using Microsoft.AspNetCore.Mvc;

// The component is implemented as below
// Valid CMATypes are course, module and activity

//@using Microsoft.AspNetCore.Identity
//@inject UserManager<ApplicationUser> userManager
//
// You put the line below where you want the component to go
//
//@await Component.InvokeAsync("UploadFile", new { userId = userManager.GetUserAsync(User).Result.Id, Id = Model.CourseId, CMAType = "Course" })

namespace Lms.MVC.UI.Views.Shared.Components.UploadFile
{
    public class FileInfo
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public string CMAType { get; set; }       
    }
    public class UploadFileViewComponent : ViewComponent
    {        
        public IUoW uow { get; set; }        

        public UploadFileViewComponent(IUoW uow)
        {
            this.uow = uow;
        }

        public IViewComponentResult Invoke(string userId, int Id, string CMAType)
        {
            // userId is for the uploader
            // Id is for the course, module or activity where the file is uploaded
            // CMAType is the type so user, course , module or activity            
            FileInfo fileInfo = new FileInfo { Id=Id, userId=userId , CMAType=CMAType};           

            return View("UploadFile",fileInfo);
        }

    }
}
