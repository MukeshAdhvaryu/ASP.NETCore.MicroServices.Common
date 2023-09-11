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
    public interface IContract
    { }
    #endregion

    #region IContract<TModelDTO, TModel, TID>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TModelDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IContract<TModelDTO, TModel, TID> : IContract, IFirstModel<TModel, TID>, IModelCount
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        , IReadable<TModelDTO, TModel, TID>
#endif
#if MODEL_DELETABLE
  , IDeleteable<TModelDTO, TModel, TID>
#endif
#if MODEL_APPENDABLE
  , IAppendable<TModelDTO, TModel, TID>
#endif
#if MODEL_UPDATABLE
  , IUpdateable<TModelDTO, TModel, TID>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
        where TModelDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
    }
    #endregion
}
