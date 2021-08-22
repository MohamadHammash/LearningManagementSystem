//TODO GitFix
using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lms.MVC.UI.Filters
{
    public class Require : ActionFilterAttribute
    {
        private readonly string name;

        public Require(string name) => this.name = name;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue(name, out Object _)) context.Result = new NotFoundResult();
        }
    }
}