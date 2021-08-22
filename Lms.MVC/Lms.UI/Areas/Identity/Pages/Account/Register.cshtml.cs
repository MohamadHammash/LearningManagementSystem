using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.Data.Repositories.Helpers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Lms.MVC.UI.Areas.Identity.Pages.Account
{
    // ToDo: gitfix
    [Authorize(Roles = "Admin , Teacher")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<RegisterModel> _logger;

        private readonly IEmailSender _emailSender;

        private readonly IUoW uoW;

        public RegisterModel(
            IUoW uoW,
             UserManager<ApplicationUser> userManager,
             SignInManager<ApplicationUser> signInManager,
             ILogger<RegisterModel> logger,
             IEmailSender emailSender)
        {
            this.uoW = uoW;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public int CourseId { get; set; }

        public List<int> Courses { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            [Remote(action: "EmailExistsCreate", controller: "ApplicationUsers")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            [StringLength(50, MinimumLength = 3)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            [StringLength(50, MinimumLength = 3)]
            public string LastName { get; set; }

            public string Name => $"{FirstName} {LastName}";

            //[Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Role :")]
            public string Role { get; set; }

            public int CourseId { get; set; }
        }

        public async Task OnGetAsync(int CourseId, string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            this.CourseId = CourseId;
        }

        public async Task<IActionResult> OnPostAsync(List<int> courses, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                if (CourseId != 0)
                {
                    courses.Add((int)CourseId);
                }

                // Input.Courses = AssignCourses(Input.Courses, Input.SelectedCourseIds);
                var password = "password";

                Input.Password = password;
                Input.ConfirmPassword = password;
                if (!User.IsInRole("Admin"))
                {
                    Input.Role = "Student";
                }
                if (courses.Last() == 0)
                {
                    courses = courses.SkipLast(1).ToList();
                }
                if (Input.Role == RoleHelper.Student && (courses.Count != 1 || courses[0] == 0))
                {
                    ModelState.AddModelError("Courses", "A student must be assigned to one course");
                }
                if (String.IsNullOrWhiteSpace(Input.Role))
                {
                    ModelState.AddModelError("Role", "Please choose a role");
                }
                if (ModelState.IsValid)
                {
                    var user = GetUserByRole(Input.Role);

                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        user.Courses = new List<Course>();

                        foreach (var item in courses)
                        {
                            user.Courses.Add(uoW.CourseRepository.GetCourseAsync(item).Result);
                        }
                        await uoW.UserRepository.ChangeRoleAsync(user);

                        var role = await _userManager.AddToRoleAsync(user, Input.Role);
                        _logger.LogInformation("User created a new account with password.");

                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        passwordToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordToken));
                        var resetPasswordCallbackUrl = Url.Page(
                            "/Account/ResetPassword",
                            pageHandler: null,
                            values: new { area = "Identity", passwordToken },
                            protocol: Request.Scheme);

                        if (!_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            await _emailSender.SendEmailAsync(
                                Input.Email,
                                "You are registered in Lms",
                                $"Your password is \"{Input.Password}\" \n Please reset your password by <a href='{HtmlEncoder.Default.Encode(resetPasswordCallbackUrl)}'>clicking here</a>.");
                        }

                        return LocalRedirect(returnUrl);

                        //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        //{
                        //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        //}
                        //else
                        //{
                        //    await _signInManager.SignInAsync(user, isPersistent: false);
                        //    return LocalRedirect(returnUrl);
                        //}
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser GetUserByRole(string role)
        {
            if (role == "Teacher" || role == "Admin")
            {
                var appUser = new ApplicationUser { UserName = $"{Input.FirstName}.{Input.LastName}", Email = Input.Email, Name = Input.Name, Role = role };
                return appUser;
            }
            else
            {
                var appUser = new ApplicationUser { UserName = $"{Input.FirstName}.{Input.LastName}", Email = Input.Email, Name = Input.Name, Role = "Student" };
                return appUser;
            }
        }

        public JsonResult OnPostCheckEmail()
        {
            var users = uoW.UserRepository.GetAllUsersAsync().Result.ToList().Select(u => u.Email).ToList(); ;
            var valid = !users.Contains(Input.Email);

            return new JsonResult(valid);
        }
    }
}