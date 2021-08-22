using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Lms.MVC.Core.Entities;

namespace Lms.MVC.UI.Models.ViewModels.ApplicationUserViewModels
{
    public class DeleteApplicationUserViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumeber { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public ICollection<Course> Courses { get; set; }
    }
}