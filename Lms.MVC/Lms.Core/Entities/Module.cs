using System;
using System.Collections.Generic;

namespace Lms.MVC.Core.Entities
{
    public class Module
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CourseId { get; set; }

        // nav prop
        public ICollection<ApplicationFile> Files { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}