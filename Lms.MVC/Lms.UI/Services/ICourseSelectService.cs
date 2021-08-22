using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lms.MVC.UI.Services
{
    public interface ICourseSelectService
    {
        Task<IEnumerable<SelectListItem>> GetTypeAsync();
    }
}