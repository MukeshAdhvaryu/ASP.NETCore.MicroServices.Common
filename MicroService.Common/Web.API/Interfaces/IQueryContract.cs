/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && MODEL_USEACTION && !MODEL_NONQUERYABLE
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Web.API.Interfaces
{
    #region IQUERY CONTRACT
    public interface IQueryContract<TModel> : IContract, IFirstModel<TModel>, IFind<TModel>
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
