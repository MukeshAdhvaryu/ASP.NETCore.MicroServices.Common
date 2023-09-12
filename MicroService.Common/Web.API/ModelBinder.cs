/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Collections;
using System.Net;
using System.Reflection;

using MicroService.Common.Constants;
using MicroService.Common.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Newtonsoft.Json;

namespace MicroService.Common.Models
{
    class ModelBinder : Binder<string>, IModelBinder
    {
        #region BIND MODEL
        async Task IModelBinder.BindModelAsync(ModelBindingContext bindingContext)
        {
            var descriptor = (ControllerActionDescriptor)bindingContext.ActionContext.ActionDescriptor;
            var controllerType = descriptor.ControllerTypeInfo;
            var modelFieldType = controllerType.GetField("service", BindingFlags.NonPublic | BindingFlags.Instance).FieldType;
            var modelType = modelFieldType.GenericTypeArguments[1];
            var model = (IExModel)Activator.CreateInstance(modelType);
            bool NeedToUseDTO = !bindingContext.ModelType.IsAssignableFrom(modelType);

            var provider = bindingContext.ValueProvider;
            BindingResultMessage[] bindingResults = new BindingResultMessage[0];

            var valueList = model.Properties.Select(p => new ValueList(provider.GetValue(p), p)).ToArray();
            var result = await Update(model, valueList);

            if (result.Item1)
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

            await response.WriteAsync(JsonConvert.SerializeObject(bindingResults));
            await response.CompleteAsync();
            return;
        }
        #endregion

        #region VALUE LIST
        struct ValueList : IValueStore<string>
        {
            readonly string name;
            readonly string firstValue;
            readonly IReadOnlyList<string> Items;
            readonly bool isEmpty;

            public ValueList(ValueProviderResult provider, string _name)
            {
                firstValue = provider.FirstValue;
                isEmpty = provider == ValueProviderResult.None;
                Items = provider.Values;
                name = _name;
            }


            public string FirstValue => firstValue;
            public int Count => Items.Count;
            public bool IsEmpty => isEmpty;

            public IEnumerator<string> GetEnumerator() =>
                Items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();

            public string this[int index] => Items[index];

            public string Name => name;
        }
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
