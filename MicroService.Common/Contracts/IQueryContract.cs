/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) ||(!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using MicroService.Common.CQRS;
#endif
//+:cnd:noEmit

using MicroService.Common.Models;

namespace MicroService.Common.Interfaces
{
    #region IQueryContract<TOutDTO, TModel, TID>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    public interface IQueryContract<TOutDTO, TModel> : IContract
    //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
        , IQuery<TOutDTO, TModel>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : ISelfModel<TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        #endregion
    { }
    #endregion
}
