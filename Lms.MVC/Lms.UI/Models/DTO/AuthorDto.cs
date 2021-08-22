using System;

namespace Lms.MVC.UI.Models.DTO
{
    public class AuthorDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public TimeSpan Age { get; set; }

        public string OrderBy { get; set; }
    }
}