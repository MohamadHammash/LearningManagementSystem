using System.ComponentModel.DataAnnotations;

using Lms.MVC.UI.Areas.Identity.Pages.Account;

using static Lms.MVC.UI.Areas.Identity.Pages.Account.RegisterModel;

namespace Lms.MVC.UI.Validations
{
    public class RegisterAttribute : ValidationAttribute
    {
        public InputModel InputModel { get; set; }

        public RegisterModel RegisterModel { get; set; }

        public RegisterAttribute(InputModel inputModel, RegisterModel registerModel)
        {
            InputModel = inputModel;
            RegisterModel = registerModel;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //const string errorMessage = "A student must be enrolled to 1 course";

            var otherPropertyInfo = validationContext.ObjectType.GetProperty(RegisterModel.Input.Role);
            var model = (RegisterModel)validationContext.ObjectInstance;

            var otherValue = otherPropertyInfo.GetValue((RegisterModel)validationContext.ObjectInstance);

            //var model = (InputModel)validationContext.ObjectInstance;
            //if (value is List<int> input)
            //{
            //if (model.Role != "Teacher" && model.Role != "Admin")
            //{
            //    if (input.Count == 1)
            //    {
            //        return ValidationResult.Success;
            //    }
            //}
            //    else if (model.Role == "Teacher" || model.Role == "Admin")
            //    {
            //        if (input.Count >= 0)
            //        {
            //            return ValidationResult.Success;
            //        }
            //    }
            //    else
            //    {
            //        return new ValidationResult(errorMessage);
            //    }
            //}
            //else
            //{
            //        return new ValidationResult(errorMessage);
            //}
            //return new ValidationResult(errorMessage);
            return ValidationResult.Success;
        }
    }
}