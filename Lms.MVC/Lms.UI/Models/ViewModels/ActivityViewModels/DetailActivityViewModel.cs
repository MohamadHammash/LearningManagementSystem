using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.ActivityViewModels
{
    public class DetailActivityViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "StartDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd, HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "EndDate")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd, HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Display(Name = "For Module")]
        public int ModuleId { get; set; }

        [Display(Name = "Module Title")]
        public string ModuleTitle { get; set; }
                public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public List<Activity> ActivityList { get; set; }

        public Microsoft.AspNetCore.Mvc.Rendering.SelectList ActivityTypes { get; set; }

        [Display(Name = "Type Of Activity")]
        public int ActivityTypeId { get; set; }

        // nav prop
        public ActivityType ActivityType { get; set; }

        public ICollection<ApplicationFile> Files { get; set; }
    }
}