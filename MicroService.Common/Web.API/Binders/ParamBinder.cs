/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

using MicroService.Common.Models;
using MicroService.Common.Parameters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MicroService.Common.Web.API
{
    public sealed class ParamBinder: ModelCreator, IModelBinder
    {
        Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
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
                    if (item == null)
                        continue;
                    JsonObject? result = JsonNode.Parse(item)?.AsObject();
                    if(result == null)
                        continue;
                    string? Name = result["name"]?.GetValue<string>()?.ToLower();
                    if (!string.IsNullOrEmpty(Name))
                    {
                        foreach (var name in PropertyNames)
                        {
                            if (Name == name.ToLower())
                            {
                                var pvalue = result["value"]?.GetValue<object>();
                                var parameter = new ObjParameter(pvalue, name);
                                Enum.TryParse(result["criteria"]?.GetValue<string>(), true, out Criteria criteria);

                                var message = model.Parse(parameter, out _, out object? value, false, criteria);
                                if (message.Status == ResultStatus.Sucess)
                                    list.Add(new SearchParameter(name, value, criteria));
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var Name = Query["name"][0]?.ToLower();

                foreach (var name in PropertyNames)
                {
                    if (Name == name.ToLower())
                    {
                        var parameter = new ModelParameter(Query["value"], name);
                        Enum.TryParse(Query["criteria"], true, out Criteria criteria);
                        var message = model.Parse(parameter, out _, out object? value, false, criteria);

                        if (message.Status == ResultStatus.Sucess)
                            list.Add(new SearchParameter(name, value, criteria));
                        break;
                    }
                }
            }
            if(list.Count ==0)
            {
                bindingContext.Result = ModelBindingResult.Success(SearchParameter.Empty);

                return Task.CompletedTask;
            }
            if (multiple)
            {
                bindingContext.Result = ModelBindingResult.Success(list);
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(list[0]);
            return Task.CompletedTask;
        }
    }
}
#endif
//+:cnd:noEmit
