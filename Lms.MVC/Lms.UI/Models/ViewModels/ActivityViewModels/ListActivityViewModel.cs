using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.ActivityViewModels
{
    public class ListActivityViewModel
    {
        public List<SchedulerEvent> events {get; set;}
    public int ModuleId { get; set; }
    public string ModuleTitle { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }
}