using System;
using System.Collections.Generic;

namespace Lms.MVC.Core.Dto
{
    public class CourseDto
    {
        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate => StartDate.AddMonths(3);

        public ICollection<ModuleDto> Modules { get; set; }
    }
}