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

    #region IContract<TModelInterface, TModel, TIDType>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IContract<TModelInterface, TModel, TIDType> : IContract, IFirstModel<TModel, TIDType>, IModelCount
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        , IReadable<TModelInterface, TModel, TIDType>
#endif
#if MODEL_DELETABLE
  , IDeleteable<TModelInterface, TModel, TIDType>
#endif
#if MODEL_APPENDABLE
  , IAppendable<TModelInterface, TModel, TIDType>
#endif
#if MODEL_UPDATABLE
  , IUpdateable<TModelInterface, TModel, TIDType>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
        where TModelInterface : IModel
        where TModel : Model<TIDType>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelInterface,
#endif
        //+:cnd:noEmit
        new()
        where TIDType : struct
        #endregion
    {
    }
    #endregion
}
