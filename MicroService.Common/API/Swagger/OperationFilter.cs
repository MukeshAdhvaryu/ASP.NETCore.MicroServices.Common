/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
Credit for the code below goes to: https://avarnon.medium.com/how-to-show-enums-as-strings-in-swashbuckle-aspnetcore-628d0cc271e6 
*/

//-:cnd:noEmit
#if !TDD && MODEL_USESWAGGER
using System.Collections;
using System.Net.Mime;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace MicroService.Common.API
{
    public class OperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var names = context.ApiDescription.ActionDescriptor.Parameters
                .Where(ad => 
                ad?.BindingInfo?.BinderType?.BaseType == typeof(Binder) &&
                !ad.ParameterType.IsAssignableTo(typeof(IEnumerable))
                ).Select(ad => ad.Name);

            if (!names.Any())
            {
                return;
            }


            foreach (var p in operation.Parameters.Where(p => names.Contains(p.Name)))
            {
                p.Content = new Dictionary<string, OpenApiMediaType>()
                {
                    [MediaTypeNames.Application.Json] = new OpenApiMediaType()
                    {
                        Schema = p.Schema
                    }
                };
                p.Schema = null;
            }
        }
    }
}
#endif
//+:cnd:noEmit
