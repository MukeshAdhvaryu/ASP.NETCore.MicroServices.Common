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
            var Request = bindingContext.ActionContext.HttpContext.Request;
            var Query = Request.Query;
            var ControllerTypeInfo = ((ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor).ControllerTypeInfo;

            #region GET CONTROLLER TYPE AND DTO TYPE
            var controllerType = ControllerTypeInfo.GetControllerType(out _);
            #endregion

            if (controllerType == null)
                goto ERROR;

            var OriginalModelName = bindingContext.OriginalModelName;

            object? Result;
            string? json;
            bool IsJson;

            #region PARAMETER IS FROM BODY
            if (Query.Count == 0)
            {
                var obj = Request.ReadFromJsonAsync(typeof(object)).Result;
                if (obj == null)
                    goto ERROR;
                json = obj.ToString();
                IsJson = true;
                goto PARSE_JSON;
            }
            #endregion

            #region PARAMETER IS FROM QUERY - SINGLE JSON STRING
            json = Query[OriginalModelName].ToString();
            IsJson = Query.ContainsKey(OriginalModelName);
            #endregion

            #region PARSE JSON
            PARSE_JSON:
            if (IsJson)
            {
                json = Query[OriginalModelName].ToString();
                Result = bindingContext.ModelType.ToSearchParam(json);

                if (Result == null)
                    goto ERROR;

                goto VALIDATE;
            }
            #endregion

            #region PARAMETER IS FROM QUERY BUT AS A COLLECTION OF STRING VALUES
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
            goto VALIDATE;
            #endregion

            #region VALIDATE
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
            #endregion

            #region RETURN SUCCESS RESULT
            bindingContext.Result = ModelBindingResult.Success(Result);
            return Task.CompletedTask;
            #endregion

            #region RETURN ERROR RESULT
            ERROR:
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
            #endregion
        }
    }
}
#endif
//+:cnd:noEmit
