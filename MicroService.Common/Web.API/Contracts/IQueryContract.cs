/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && MODEL_USEACTION 
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) || (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using MicroService.Common.Web.API.CQRS;
#endif
//+:cnd:noEmit

namespace MicroService.Common.Web.API.Interfaces
{
    #region IQUERY CONTRACT
    public interface IQueryContract<TModel> : IContract
    //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
        , IActionQuery<TModel>
#endif
        //+:cnd:noEmit

        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TModel>,
        new()
        #endregion
    { }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
