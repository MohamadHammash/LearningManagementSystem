using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

using Microsoft.AspNetCore.Mvc;

namespace Lms.MVC.UI.Models.ViewModels.ApplicationUserViewModels
{
    public class EditApplicationUserViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [EmailAddress]
        [Remote("EmailExistsEdit", "ApplicationUsers", AdditionalFields = ("Id"))]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Role { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}