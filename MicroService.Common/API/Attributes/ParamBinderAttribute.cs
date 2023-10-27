/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD && MODEL_SEARCHABLE && (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)

namespace MicroService.Common.API
{
    public class ParamBinderAttribute : FlexiBinderAttribute<ParamBinder>
    { }      
}
#endif
//+:cnd:noEmit
