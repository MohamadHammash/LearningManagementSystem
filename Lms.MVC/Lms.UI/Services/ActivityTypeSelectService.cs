using System.Collections.Generic;
using System.Linq;

using Lms.MVC.Core.Repositories;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lms.MVC.UI.Services
{
    public class ActivityTypeSelectService : IActivityTypeSelectService
    {
        private readonly IUoW uoW;

        public ActivityTypeSelectService(IUoW uoW)
        {
            this.uoW = uoW;
        }

        public IEnumerable<SelectListItem> GetTypeAsync()
        {
            var activityTypes = uoW.ActivityRepository.GetAllActivityTypesAsync().Result;

            return activityTypes.Select(c => new SelectListItem
            {
                Text = c.Name.ToString(),
                Value = c.Id.ToString()
            }).Distinct();
        }
    }
}