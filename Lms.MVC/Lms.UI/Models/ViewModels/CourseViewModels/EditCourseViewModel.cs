using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.CourseViewModels
{
    public class EditCourseViewModel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        // nav prop
        public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<Module> Modules { get; set; }

        public ICollection<ApplicationFile> Files { get; set; }
    }
}