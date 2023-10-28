/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Text.Json;

using MicroService.Common.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            var Request = bindingContext.ActionContext.HttpContext.Request;
            var Query = Request.Query;
            var ControllerTypeInfo = ((ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor).ControllerTypeInfo;

            #region GET CONTROLLER TYPE AND DTO TYPE
            var controllerType = ControllerTypeInfo.GetControllerType(out Type? inDTOType);
            #endregion

            if (inDTOType == null || controllerType == null)
                goto ERROR;

            var OriginalModelName = bindingContext.OriginalModelName;

            object? Result;
            string? json;
            bool IsJson;

            #region DTO IS FROM BODY
            if (Query.Count == 0)
            {
                var obj = Request.ReadFromJsonAsync(typeof(object)).Result;
                if(obj == null)
                    goto ERROR;
                json = obj.ToString();
                IsJson = true;
                goto PARSE_JSON;
            }
            #endregion

            #region DTO IS FROM QUERY - SINGLE JSON STRING
            json = Query[OriginalModelName].ToString();
            IsJson = Query.ContainsKey(OriginalModelName);
            #endregion

            #region PARSE JSON
            PARSE_JSON:
            if (IsJson)
            {
                Result = bindingContext.ModelType.ToDTO(json, controllerType);

                if (Result == null)
                    goto ERROR;

                goto VALIDATE;
            }
            #endregion

            #region DTO IS FROM QUERY BUT AS A COLLECTION OF STRING VALUES
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
            #endregion

            #region VALIDATE RESULT
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
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
