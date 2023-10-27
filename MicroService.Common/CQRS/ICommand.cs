/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.CQRS
{
    #region ICommand<TOutDTO, TModel, TID>
    //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface ICommand<TOutDTO, TModel, TID> : IModelCount
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        , IAppendable<TOutDTO, TModel, TID>
#endif
#if MODEL_UPDATABLE
        , IUpdatable<TOutDTO, TModel, TID>
#endif
#if MODEL_DELETABLE
        , IDeleteable<TOutDTO, TModel, TID>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
        where TOutDTO : IModel, new()
        where TModel : class, ISelfModel<TID, TModel>, new()
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        , TOutDTO
#endif
        //+:cnd:noEmit
        where TID : struct
        #endregion
    {
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IExCommand<TOutDTO, TModel, TID>
    //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    internal interface IExCommand<TOutDTO, TModel, TID> : ICommand<TOutDTO, TModel, TID>, IFirstModel<TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel, new()
        where TModel : class, ISelfModel<TID, TModel>, new()
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        , TOutDTO
#endif
        //+:cnd:noEmit
        where TID : struct
        #endregion
    {
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
        IQuery<TOutDTO, TModel, TID> GetQueryObject();
#endif
        //+:cnd:noEmit
    }
#endif
    //+:cnd:noEmit
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
