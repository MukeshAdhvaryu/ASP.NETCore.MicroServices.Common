/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD && MODEL_SEARCHABLE && (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MicroService.Common.API
{
    public class ParamBinderAttribute : FromQueryAttribute, IModelNameProvider, IBinderTypeProviderMetadata
    {
        ModelBinderAttribute binder = new ModelBinderAttribute();

        public ParamBinderAttribute()
        {
            binder.BinderType = typeof(ParamBinder);
        }
        string? IModelNameProvider.Name => binder.Name;
        Type? IBinderTypeProviderMetadata.BinderType => binder.BinderType;
        BindingSource? IBindingSourceMetadata.BindingSource => binder.BindingSource;
    }      
}
#endif
//+:cnd:noEmit
