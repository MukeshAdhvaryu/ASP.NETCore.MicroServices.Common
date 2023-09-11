/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

#if !TDD
using MicroService.Common.Models;
using MicroService.Common.Parameters;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MicroService.Common.Web.API
{
    public sealed class ParamBinder: ModelCreator, IModelBinder
    {
        async Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var descriptor = (ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor;
            var model = (IExModel)GetModel(descriptor.ControllerTypeInfo, out _);

            var Query = bindingContext.ActionContext.HttpContext.Request.Query;
            bool multiple = Query.ContainsKey(bindingContext.OriginalModelName);

            List<SearchParameter> list = new List<SearchParameter>();
            var PropertyNames = model.GetPropertyNames();

            if (multiple)
            {
                var items = Query[bindingContext.OriginalModelName];
                foreach (var item in items)
                {
                    //working on it. in next version....
                }

            }
            else
            {
                foreach (var name in PropertyNames)
                {
                    if (Query["name"][0].ToLower() == name.ToLower())
                    {
                        var parameter = new ModelParameter(Query["value"], name);
                        var message = model.Parse(parameter, out _, out object value, false);
                        if (message.Status == ResultStatus.Sucess)
                        {
                            Enum.TryParse(Query["criteria"], out Criteria criteria);
                            Enum.TryParse(Query["andor"], out AndOr andOr);

                            list.Add(new SearchParameter(name, value, criteria, andOr));
                        }
                    }
                }
            }
            if(multiple)
            {
                bindingContext.Result = ModelBindingResult.Success(list);
                return;
            }
            bindingContext.Result = ModelBindingResult.Success(list.FirstOrDefault());
        }
    }
}
#endif
