/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD  
using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common.API
{
    public class DTOBinderAttribute : FlexiBinderAttribute<ModelBinder>
    { }
}
#endif
//+:cnd:noEmit
