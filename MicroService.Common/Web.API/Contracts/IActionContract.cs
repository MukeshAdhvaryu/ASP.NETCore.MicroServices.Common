/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && MODEL_USEACTION
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Web.API.Interfaces
{
    public interface IActionContract<TModel, TID>: IContract
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
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        , IFirstModel<TModel>
        , IFetch<TModel>
        , IFindByID<TModel, TID>
#if MODEL_SEARCHABLE
        , ISearch<TModel>
#endif
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
