/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && MODEL_USEACTION && (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Web.API.Interfaces;

namespace MicroService.Common.Web.API.CQRS
{
    public interface IActionCommand<TModel, TID> : IContract
  //-:cnd:noEmit
#if MODEL_DELETABLE
  , IDeleteable<TModel, TID>
#endif
#if MODEL_APPENDABLE
  , IAppendable<TModel, TID>
#endif
#if MODEL_UPDATABLE
  , IUpdatable<TModel, TID>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
