/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD && MODEL_SEARCHABLE && (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using System.Net;
using System.Xml.Linq;

using MicroService.Common.Constants;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MicroService.Common.API
{
    public sealed class ParamBinder: Binder
    {
        #region CONSTRUCTORS
        public ParamBinder(IObjectModelValidator _validator) :
            base(_validator)
        { }
        #endregion

        public override Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var descriptor = (ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor;
            var Query = bindingContext.ActionContext.HttpContext.Request.Query;

            var controllerType = descriptor.ControllerTypeInfo.GetControllerType(out _);

            if (controllerType == null || Query == null || Query.Count == 0)
                goto ERROR;

            var OriginalModelName = bindingContext.OriginalModelName;
            object? Result;

            bool IsJson = Query.ContainsKey(OriginalModelName);
            if (IsJson)
            {
                var json = Query[OriginalModelName].ToString();
                Result = bindingContext.ModelType.ToSearchParameter(json);

                if (Result == null)
                    goto ERROR;

                goto VALIDATE;
            }

            IExModel Model = (IExModel)controllerType.GetModel(true);
            var propertyName = Query["name"][0]?.ToLower();
            if (string.IsNullOrEmpty(propertyName))
                goto ERROR;

            Enum.TryParse(Query["criteria"], true, out Criteria criteria);
            var items = Query["value"]; 

            if (!Model.Parse(propertyName, items, out Result))
            {
                goto ERROR;
            }

            Result = new SearchParameter(propertyName, criteria, Result);

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
    }
}
#endif
//+:cnd:noEmit
