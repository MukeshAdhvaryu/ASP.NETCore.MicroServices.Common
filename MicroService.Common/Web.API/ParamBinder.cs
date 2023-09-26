/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
using System;

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

            List<ISearchParameter> list = new List<ISearchParameter>();
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
                            if (Query.ContainsKey("andOr"))
                            {
                                Enum.TryParse(Query["andor"], out AndOr andOr);
                                list.Add(new MultiSearchParameter(name, value, criteria, andOr));
                            }
                            else
                            {
                                list.Add(new SearchParameter(name, value, criteria));
                            }
                        }
                    }
                }
            }
            if(list.Count ==0)
            {
                if(bindingContext.ModelType == typeof(IMultiSearchParameter)) 
                    bindingContext.Result = ModelBindingResult.Success(MultiSearchParameter.Empty);
                else
                    bindingContext.Result = ModelBindingResult.Success(SearchParameter.Empty);

                return;
            }
            if (multiple)
            {
                bindingContext.Result = ModelBindingResult.Success(list);
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(list[0]);
        }
    }
}
#endif
//+:cnd:noEmit
