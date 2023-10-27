/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD 
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MicroService.Common.API
{    public abstract class FlexiBinderAttribute<T> :
     //-:cnd:noEmit
#if MODEL_FROMQUERY
    FromQueryAttribute, IModelNameProvider, IBinderTypeProviderMetadata
#else
     ModelBinderAttribute
#endif
    //+:cnd:noEmit
    where T: IModelBinder
    {
        //-:cnd:noEmit
#if MODEL_FROMQUERY
        ModelBinderAttribute binder;
#endif
        //+:cnd:noEmit
        protected FlexiBinderAttribute()
        {
            //-:cnd:noEmit
#if MODEL_FROMQUERY
            binder = new ModelBinderAttribute(typeof(T));
#else
            BinderType = typeof(T);
#endif
            //+:cnd:noEmit
        }

        //-:cnd:noEmit
#if MODEL_FROMQUERY
        public string? Name
        {
            get => binder.Name;
            set => binder.Name = value;
        }
        public Type? BinderType => binder.BinderType;
#endif
        //+:cnd:noEmit
    }
}
#endif
//+:cnd:noEmit
