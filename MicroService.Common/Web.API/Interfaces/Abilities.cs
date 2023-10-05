/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && MODEL_USEACTION
//+:cnd:noEmit

using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Services;

using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common.Web.API.Interfaces
{
    #region IReadable<TModel, TID>
    //-:cnd:noEmit
#if !MODEL_NONREADABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IReadable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Get(TID? id);

        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> GetAll(int limitOfResult = 0);

        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> GetAll(int startIndex, int limitOfResult);

        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0);

        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> FindAll(ISearchParameter? parameter);

        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IDeletable<TModel, TID>
    //-:cnd:noEmit
#if MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows deleting a single model with a specified ID.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IDeleteable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Delete(TID id);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IAppendable<TModel, TID>
    //-:cnd:noEmit
#if MODEL_APPENDABLE
    /// <summary>
    /// This interface represents an object that has a list of models to which a new model can be appended.
    /// Any object that implements the IModel interface can be provided. This allows DTOs to be used instead of an actual model object.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IAppendable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Adds a new model based on the given model.
        /// If the given model is not TModel, then a new appropriate model will be created by copying data from the given model.
        /// </summary>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Add(IModel? model);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IUpdatable<TModel, TID>
    //-:cnd:noEmit
#if MODEL_UPDATABLE
    /// <summary>
    /// This interface represents an object that has a list of models and allows a model with a specified ID to be updated with data from the given model parameter.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IUpdatable<TModel, TID>
        #region TYPE CONSTRINTS
        where TModel : ISelfModel<TID, TModel>,
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Updates a model specified by the given ID with the data of the given model.
        /// </summary>
        /// <param name="id">ID of the model to be updated.</param>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>An instance of IActionResult.</returns>
        Task<IActionResult> Update(TID id, IModel? model);
    }
#endif
    //+:cnd:noEmit
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
