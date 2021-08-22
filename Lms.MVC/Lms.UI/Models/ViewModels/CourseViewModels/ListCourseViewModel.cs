using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.CourseViewModels
{
    public class ListCourseViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public bool ShowOnlyMyCourses { get; set; }

        // nav prop
        public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<Module> Modules { get; set; }

        public ICollection<ApplicationFile> Files { get; set; }
    }
}