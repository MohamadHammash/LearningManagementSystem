using System;
using System.Collections.Generic;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.ActivityViewModels
{
    public class DetailsActivityViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ModuleId { get; set; }

        public int ActivityTypeId { get; set; }

        // nav prop
        public ActivityType ActivityType { get; set; }

        public ICollection<ApplicationFile> Files { get; set; }
    }
}