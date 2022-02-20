using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SWMichigan.WebAPIs.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            HttpResponseMessage response = null;
            if (context.Exception is UnauthorizedAccessException)
            {
                response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized Access."),
                    ReasonPhrase = "Unauthorized Access. Please Contact your Administrator.",
                    StatusCode = HttpStatusCode.Unauthorized
                };
                // handle logging here
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Something went wrong.Please Contact your Administrator."),
                    ReasonPhrase = "Internal Server Error. Please Contact your Administrator.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
                // Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";                
                string stack = null;
#else
                //    var msg = context.Exception.GetBaseException().Message;
                //   string stack = context.Exception.StackTrace;
#endif
            }

            //log this exception message to the file or database.
            Log(context);
            context.Result = new JsonResult(response);
        }

        private void Log(ExceptionContext context)
        {
           
             
        }
    }
}
