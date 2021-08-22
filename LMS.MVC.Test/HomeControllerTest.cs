using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Lms.MVC.Test
{
    public class HomeControllerTest
    {
        private readonly Mock<IUoW> MockUow;

        private readonly HomeController controller;

        public HomeControllerTest()
        {
            MockUow = new Mock<IUoW>();
            controller = new HomeController(MockUow.Object);
        }

        [Fact]
        public void HomeControllerReturnsAViewIfUserIsAdmin()
        {           
            var mockUser = new Mock<ApplicationUser>();
            var userList = new List<ApplicationUser>() { mockUser.Object };
            IEnumerable<ApplicationUser> userEnumerable = userList;

            MockUow.Setup(m => m.UserRepository.GetAllUsersAsync()).Returns(Task.FromResult(userEnumerable));

            var mockCourse = new Mock<Course>();
            var courseList = new List<Course>() { mockCourse.Object };
            IEnumerable<Course> courseEnumerable = courseList;            
            MockUow.Setup(m => m.CourseRepository.GetAllCoursesAsync(false,false)).Returns(Task.FromResult(courseEnumerable));

            var mockModule = new Mock<Module>();
            var moduleList = new List<Module>() { mockModule.Object };
            IEnumerable<Module> moduleEnumerable = moduleList;
            MockUow.Setup(m => m.ModuleRepository.GetAllModulesAsync(false)).Returns(Task.FromResult(moduleEnumerable));

            var mockActivity = new Mock<Activity>();
            var activityList = new List<Activity>() { mockActivity.Object };
            IEnumerable<Activity> activityEnumerable = activityList;
            MockUow.Setup(m => m.ActivityRepository.GetAllActivitiesAsync()).Returns(Task.FromResult(activityEnumerable));

            controller.ControllerContext = new ControllerContext();

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, "TestUser"),
                     new Claim(ClaimTypes.Role, "Admin")
                }))
            };

            controller.ControllerContext.HttpContext = context;

            var viewResult = (ViewResult)controller.Index();
            Assert.NotNull(viewResult);
            Assert.IsType<ViewResult>(viewResult);
        }

        [Fact]
        public void HomeControllerReturnsAViewIfUserIsTeacher()
        {    
            controller.ControllerContext = new ControllerContext();

          var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, "TestUser"),
                     new Claim(ClaimTypes.Role, "Teacher")
                }))
            };

            controller.ControllerContext.HttpContext = context;

            var viewResult = controller.Index();
            Assert.NotNull(viewResult);
            Assert.IsType<RedirectToActionResult>(viewResult);           
        }

        [Fact]
        public void HomeControllerReturnsAViewIfUserIsStudent()
        {                          
            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, "TestUser"),
                     new Claim(ClaimTypes.Role, "Student")
                }))
            };

            controller.ControllerContext.HttpContext = context;

            var viewResult = controller.Index();
            Assert.NotNull(viewResult);
            Assert.IsType<RedirectToActionResult>(viewResult);            
        }

        [Fact]
        public void HomeControllerReturnsANullIfUserIsARoleThatIsNotRegistered()
        {            
            controller.ControllerContext = new ControllerContext();

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, "TestUser"),
                     new Claim(ClaimTypes.Role, "ARoleThatDoesntExist")
                }))
            };

            controller.ControllerContext.HttpContext = context;

            var viewResult = controller.Index();
            Assert.Null(viewResult);            
        }        

        [Fact]
        public void PrivacyReturnsAView()
        {
            var controller = new HomeController(MockUow.Object);                        

            var viewResult = controller.Privacy();
            Assert.NotNull(viewResult);
        }        
    }
}