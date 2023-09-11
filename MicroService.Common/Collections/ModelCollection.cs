/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Collections;
using System.Reflection;

using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

namespace MicroService.Common.Collections
{
    #region IModelCollection
    public interface IModelCollection
    { }
    #endregion

    #region IModelCollection<TModel, TID>
    /// <summary>
    /// Represents an object which holds a collection of models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    /// <typeparam name="TID">Type of TID</typeparam>
    public partial interface IModelCollection<TModel, TID> : IModelCollection, IFirstModel<TModel, TID>, IModelCount
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        , IEnumerable<TModel>
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        , IModifiable
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : Model<TID>
        where TID : struct
        #endregion
    {
        #region FIND
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Finds a model based on given keys.
        /// </summary>
        /// <param name="keys">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        Task<TModel?> Find(IEnumerable<ISearchParameter> keys);

        /// <summary>
        /// Finds all models matched based on given key.
        /// </summary>
        /// <param name="key">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        Task<IEnumerable<TModel>> FindAll(ISearchParameter key);

        /// <summary>
        /// Finds a model based on given keys.
        /// </summary>
        /// <param name="keys">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        Task<TModel?> Find(TID id);
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        /// <summary>
        /// Adds a specified model.
        /// </summary>
        /// <param name="model">Model to add.</param>
        /// <returns>Task with result of type boolean.</returns>
        Task<bool> Add(TModel model);

        /// <summary>
        /// Adds a range of specified models.
        /// </summary>
        /// <param name="models">Models to add.</param>
        /// <returns>Task with result of type boolean.</returns>
        Task<bool> AddRange(IEnumerable<TModel> models);
#endif
        //+:cnd:noEmit
        #endregion

        #region DELTE
        //-:cnd:noEmit
#if MODEL_DELETABLE
        /// <summary>
        /// Adds a specified model.
        /// </summary>
        /// <param name="model">Model to delete.</param>
        /// <returns>Task with result of type boolean.</returns>
        Task<bool> Delete(TModel model);

        /// <summary>
        /// Deletes a range of specified models.
        /// </summary>
        /// <param name="models">Models to delete.</param>
        /// <returns>Task with result of type boolean.</returns>
        Task<bool> DeleteRange(IEnumerable<TModel> models);
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE
        //-:cnd:noEmit
#if MODEL_UPDATABLE
        /// <summary>
        /// Updates a specified model.
        /// </summary>
        /// <param name="model">Model to update.</param>
        /// <returns>Task with result of type boolean.</returns>
        Task<bool> Update(TModel model);

        /// <summary>
        /// Updates a range of specified models.
        /// </summary>
        /// <param name="models">Models to update.</param>
        /// <returns>Task with result of type boolean.</returns>
        Task<bool> UpdateRange(IEnumerable<TModel> models);
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
