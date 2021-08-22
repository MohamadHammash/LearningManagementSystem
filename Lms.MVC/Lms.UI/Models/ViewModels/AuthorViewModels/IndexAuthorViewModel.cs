using System.ComponentModel.DataAnnotations;

namespace Lms.MVC.UI.Models.ViewModels.AuthorViewModels
{
    public class IndexAuthorViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public int Age { get; set; }

        public string OrderBy { get; set; }
    }
}