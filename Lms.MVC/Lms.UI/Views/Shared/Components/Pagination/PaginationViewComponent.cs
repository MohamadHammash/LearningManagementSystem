using System.Threading.Tasks;

using Lms.MVC.UI.Utilities.Pagination;

using Microsoft.AspNetCore.Mvc;

namespace Lms.MVC.UI.Views.Shared.Components.Pagination
{
    public class PaginationViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PaginationResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}