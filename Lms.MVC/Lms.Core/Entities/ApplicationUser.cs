using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace Lms.MVC.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }

        public string Role { get; set; }

        // If a student its the course the student takes. If a teacher its the courses the teacher teaches
        public ICollection<Course> Courses { get; set; }

        // nav prop
        public ICollection<ApplicationFile> Files { get; set; }
    }
}