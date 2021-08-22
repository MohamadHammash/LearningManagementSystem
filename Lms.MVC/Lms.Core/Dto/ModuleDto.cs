using System;

namespace Lms.MVC.Core.Dto
{
    public class ModuleDto
    {
        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate => StartDate.AddMonths(1);
    }
}