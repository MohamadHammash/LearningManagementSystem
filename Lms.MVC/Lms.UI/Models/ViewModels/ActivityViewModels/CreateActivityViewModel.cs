using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.ActivityViewModels
{
    public class CreateActivityViewModel
    {
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "StartDate")]

        // TODO: Why does displayformat break the default value set in the controller
        [DataType(DataType.DateTime), Required]
        public DateTime StartDate { get; set; }

        [DataType(DataType.DateTime), Required]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd, HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "EndDate")]
        public DateTime EndDate { get; set; }

        [Display(Name = "For Module")]
        public int ModuleId { get; set; }

        [Display(Name = "Module Title")]
        public string ModuleTitle { get; set; }

        public List<Activity> ActivityList { get; set; }

        public Microsoft.AspNetCore.Mvc.Rendering.SelectList ActivityTypes { get; set; }

        [Display(Name = "Type Of Activity")]
        public int ActivityTypeId { get; set; }

        // nav prop
        public ActivityType ActivityType { get; set; }

        public ICollection<ApplicationFile> Files { get; set; }
    }
}