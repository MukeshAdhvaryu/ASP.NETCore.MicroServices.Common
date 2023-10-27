/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using MicroService.Common.Models;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MicroService.Common.API
{
    public sealed class ModelBinder : Binder
    {
        #region CONSTRUCTORS
        public ModelBinder(IObjectModelValidator _validator) :
            base(_validator)
        { }
        #endregion

        #region BIND MODEL
        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var descriptor = (ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor;
            var Query = bindingContext.ActionContext.HttpContext.Request.Query;

            var controllerType = descriptor.ControllerTypeInfo.GetControllerType(out Type? inDTOType);

            if (inDTOType == null || controllerType == null || Query == null || Query.Count == 0)
                goto ERROR;

            var OriginalModelName = bindingContext.OriginalModelName;

            object? Result;

            bool IsJson = Query.ContainsKey(OriginalModelName);
            if (IsJson)
            {
                var json = Query[OriginalModelName].ToString();
                Result = bindingContext.ModelType.ToDTO(json, controllerType);

                if (Result == null)
                    goto ERROR;

                goto VALIDATE;
            }

            IExModel Model = (IExModel)controllerType.GetModel(Query.ContainsKey(OriginalModelName));
            bool NeedToUseDTO = !inDTOType.IsAssignableFrom(Model.GetType());

            foreach (var propertyName in Query.Keys)
            {
                var items = Query[propertyName];
                if (!Model.Parse(propertyName, items, out _, true))
                    goto ERROR;
            }

            //-:cnd:noEmit
#if MODEL_USEDTO
            if (NeedToUseDTO)
            {
                Result = Model.ToDTO(inDTOType);
                goto VALIDATE;
            }
#endif
            //+:cnd:noEmit

            Result = Model;
            goto VALIDATE;

            VALIDATE:
            if (Result != null)
            {
                Validator.Validate(
                    bindingContext.ActionContext,
                    validationState: bindingContext.ValidationState,
                    prefix: string.Empty,
                    model: Result
                );
            }
            bindingContext.Result = ModelBindingResult.Success(Result);
            return Task.CompletedTask;

            ERROR:
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
