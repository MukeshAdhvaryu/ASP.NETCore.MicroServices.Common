/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Net;

using MicroService.Common.Constants;
using MicroService.Common.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MicroService.Common.Web.API
{
    public sealed class ModelBinder : ModelCreator, IModelBinder
    {
        #region BIND MODEL
        async Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var descriptor = (ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor;
            var model = (IExModel)GetModel(descriptor.ControllerTypeInfo, out Type modelType);

            bool NeedToUseDTO = !bindingContext.ModelType.IsAssignableFrom(modelType);

            var Query = bindingContext.ActionContext.HttpContext.Request.Query;

            List<Message> messages = new List<Message>();
            var PropertyNames = model.GetPropertyNames();
            bool failed = false;

            foreach ( var name in PropertyNames )
            {
                var parameter = Query.ContainsKey(name) ? new ModelParameter(Query[name], name) : new ModelParameter(name);
                var message = model.Parse(parameter, out _, out _, true);

                switch (message.Status)
                {
                    case ResultStatus.Failure:
                    case ResultStatus.MissingRequiredValue:
                        failed = true;
                        break;
                    default:
                        break;
                }
                messages.Add(message);
            }            
            if (!failed)
            {
                //-:cnd:noEmit
#if MODEL_USEDTO
                if (NeedToUseDTO)
                    bindingContext.Result = ModelBindingResult.Success(model.ToDTO(bindingContext.ModelType));
                else
                    bindingContext.Result = ModelBindingResult.Success(model);

#else
                bindingContext.Result = ModelBindingResult.Success(model);

#endif
                //+:cnd:noEmit
                return;
            }

            var response = bindingContext.HttpContext.Response;
            response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            response.ContentType = Contents.JSON;
            await response.WriteAsJsonAsync(messages.Select((m, i) => ++i+ ". " + m));
            await response.CompleteAsync();
            return;
        }
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
