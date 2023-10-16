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
    #region IQueryActionContract<TModel>
    public interface IActionQueryContract<TModel> : IContract
    //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
        , IFirstModel<TModel>
        , IFetch<TModel>
#if MODEL_SEARCHABLE
        , ISearch<TModel>
#endif
#endif
        //+:cnd:noEmit

        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TModel>,
        new()
        #endregion
    { }
    #endregion

    #region IQueryContract<TModel, TID>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    public interface IActionQueryContract<TModel, TID> : IActionQueryContract<TModel>
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
        , IFindByID<TModel, TID>
#endif
        //+:cnd:noEmit

        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    { }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
