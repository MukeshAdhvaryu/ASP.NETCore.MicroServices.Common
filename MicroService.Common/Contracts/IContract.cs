/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Models;

namespace MicroService.Common.Interfaces
{
    #region IContract
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    public interface IContract : IModelCount, IFirstModel
    { }
    #endregion

    #region IContract<TOutDTO, TModel, TID>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IContract<TOutDTO, TModel, TID> : IContract, IFirstModel<TModel, TID>, IModelCount
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        , IReadable<TOutDTO, TModel, TID>
#endif
#if MODEL_DELETABLE
        , IDeleteable<TOutDTO, TModel, TID>
#endif
#if MODEL_APPENDABLE
        , IAppendable<TOutDTO, TModel, TID>
#endif
#if MODEL_UPDATABLE
        , IUpdatable<TOutDTO, TModel, TID>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
    }
    #endregion
}
