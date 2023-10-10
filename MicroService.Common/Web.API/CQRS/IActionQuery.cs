/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && MODEL_USEACTION && (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Web.API.Interfaces;

namespace MicroService.Common.Web.API.CQRS
{
    public interface IActionQuery<TModel> : IContract, IFind<TModel>, IFirstModel<TModel>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TModel>,
        new()
        #endregion
    {
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
