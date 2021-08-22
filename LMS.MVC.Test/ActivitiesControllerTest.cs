using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Lms.MVC.Core.Entities;
using Lms.MVC.Core.Repositories;
using Lms.MVC.UI.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Moq;

using Xunit;

namespace Lms.MVC.Test
{
    public class ActivitiesControllerTest
    {
        private readonly Mock<IUoW> MockUow;
        private readonly Mock<IMapper> MockMapper;

        private readonly ActivitiesController controller;

        public ActivitiesControllerTest()
        {
            MockMapper = new Mock<IMapper>();
            MockUow = new Mock<IUoW>();
            controller = new ActivitiesController(MockMapper.Object, MockUow.Object);
        }

        [Fact]
        public void ActivitiesControllerReturnsAViewIfIdIsNotNull()
        {            
            var mockModule = new Mock<Module>();
            mockModule.Object.Title="TestTitlee";
            MockUow.Setup(m => m.ModuleRepository.GetModuleAsync(It.IsAny<int>())).Returns(Task.FromResult(mockModule.Object));

            var mockActivity = new Mock<Activity>();
            var activityList = new List<Activity>() { mockActivity.Object };
            MockUow.Setup(m => m.ActivityRepository.GetAllActivitiesByModuleIdAsync(It.IsAny<int>())).Returns(Task.FromResult(activityList));

            var viewResult = controller.Index(4).Result;
            Assert.NotNull(viewResult);
            Assert.IsType<ViewResult>(viewResult);
        }

        [Fact]
        public void ActivitiesControllerReturnsAViewIfIdIsNullAndUserIsAStudent()
        {
            controller.ControllerContext = new ControllerContext();

            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.Name, "TestUser"),
                     new Claim(ClaimTypes.Role, "Student")
                }))
            };           
            controller.ControllerContext.HttpContext = context;

            var viewResult = controller.Index(null).Result;
            Assert.NotNull(viewResult);
            Assert.IsType<RedirectToActionResult>(viewResult);
        }

        [Fact]
        public void ActivitiesControllerReturnsNullIfIdIsNullAndUserIsNotAStudent()
        {
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

            var viewResult = controller.Index(null).Result;
            Assert.NotNull(viewResult);
            Assert.IsType<RedirectToActionResult>(viewResult);
        }
    }
}