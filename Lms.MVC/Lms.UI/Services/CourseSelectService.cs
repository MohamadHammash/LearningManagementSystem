using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lms.MVC.Core.Repositories;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lms.MVC.UI.Services
{
    public class CourseSelectService : ICourseSelectService
    {
        private readonly IUoW uoW;

        public CourseSelectService(IUoW uoW)
        {
            this.uoW = uoW;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypeAsync()
        {
            var courses = await uoW.CourseRepository.GetAllCoursesAsync(false);

            return courses.Select(c => new SelectListItem
            {
                Text = c.Title.ToString(),
                Value = c.Id.ToString()
            }).Distinct();
        }
    }
}