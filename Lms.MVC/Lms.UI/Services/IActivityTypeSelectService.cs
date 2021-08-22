using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lms.MVC.UI.Services
{
    public interface IActivityTypeSelectService
    {
        IEnumerable<SelectListItem> GetTypeAsync();
    }
}