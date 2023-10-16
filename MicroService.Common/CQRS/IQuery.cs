/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.CQRS
{
    #region IQuery<TOutDTO, TModel>
    /// <summary>
    /// Represents an object which holds a enumerables of keyless models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model></typeparam>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    public partial interface IQuery<TOutDTO, TModel> : IModelCount, IFirstModel<TModel>, 
        IFetch<TOutDTO, TModel>
        //-:cnd:noEmit
#if MODEL_SEARCHABLE
        , ISearch<TOutDTO, TModel>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        where TOutDTO : IModel
        #endregion
    { }
    #endregion

    #region IQuery<TOutDTO, TModel, TID>
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IQuery<TOutDTO, TModel, TID> : IQuery<TOutDTO, TModel>,
        IFindByID<TOutDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
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
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
