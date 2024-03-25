using ESD.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace DAS.Web.Attributes
{
    public class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public readonly int[] Permissions; //Permission string to get from controller
        public readonly int CodeModule;
        public HasPermissionAttribute(int codeModule, int[] permission)
        {
            CodeModule = codeModule;
            Permissions = permission;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IServiceProvider services = context.HttpContext.RequestServices;
            var authorize = (IAuthorizeService)services.GetService(typeof(IAuthorizeService));
            var taskIsAccess = Task.Run(() => authorize.CheckPermission(CodeModule, Permissions));
            taskIsAccess.Wait();
            var isAccess = taskIsAccess.Result;
            if (!isAccess)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
                return;
            }
        } 
    }
}
