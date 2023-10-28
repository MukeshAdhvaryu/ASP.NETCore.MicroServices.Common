/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MicroService.Common.API
{
    public abstract class FlexiBinderAttribute<T> :
     //-:cnd:noEmit
#if MODEL_FROMQUERY
    FromQueryAttribute
#elif MODEL_FROMBODY
    FromBodyAttribute
#else
    ModelBinderAttribute
#endif
    , IModelNameProvider, IBinderTypeProviderMetadata
    //+:cnd:noEmit
    where T: IModelBinder
    {
        protected FlexiBinderAttribute() 
        { 
        }

        Type? IBinderTypeProviderMetadata.BinderType => typeof(T);

        //-:cnd:noEmit
#if MODEL_FROMBODY
       public string? Name {get ;set ;}
#endif
        //+:cnd:noEmit
    }
}
#endif
//+:cnd:noEmit
