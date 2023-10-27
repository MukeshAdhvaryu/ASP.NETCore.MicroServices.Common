/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Net;

using MicroService.Common.Constants;
using MicroService.Common.Exceptions;
using MicroService.Common.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MicroService.Common.API.Middlewares
{
    public class HttpExceptionFilter : IExceptionFilter
    {
        void IExceptionFilter.OnException(ExceptionContext context)
        {
            ProblemDetails problem;

            if (context.Exception is IModelException)
            {
                var modelException = ((IModelException)context.Exception);                 

                modelException.GetConsolidatedMessage(out string title, out string type, out string? details, out string[]? stackTrace, Globals.IsProductionEnvironment);
                problem = new ProblemDetails()
                {
                    Type = ((HttpStatusCode)modelException.Status).ToString() + ": " + type, // customize
                    Title = title, //customize
                    Status = modelException.Status, //customize
                    Detail = details,
                };
                var response = context.HttpContext.Response;
                response.ContentType = Contents.JSON;
                response.StatusCode = modelException.Status;
                if(stackTrace != null)
                {
                    response.WriteAsJsonAsync(new object[] { problem, stackTrace }).Wait();
                }
                else
                {
                    response.WriteAsJsonAsync(problem).Wait();
                }
                response.CompleteAsync().Wait();
                return;
            }
            
            problem = new ProblemDetails()
            {
                Type = "Error", // customize
                Title = "Error", //customize
                Status = (int)HttpStatusCode.ExpectationFailed, //customize
                Detail = context.Exception.Message,
            };
            context.Result = new JsonResult(problem);
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
