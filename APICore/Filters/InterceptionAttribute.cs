using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WaveCityCenterAPI.Core.Filters
{
    public class InterceptionAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Log(actionExecutedContext.ActionContext.RequestContext.RouteData);

            base.OnActionExecuted(actionExecutedContext);
        }

        private void Log(System.Web.Http.Routing.IHttpRouteData httpRouteData)
        {
            var controllerName = "controller name";
            var actionName = "action name";
            var message = String.Format("controller:{0}, action:{1}", controllerName, actionName);

        }
    }
}
