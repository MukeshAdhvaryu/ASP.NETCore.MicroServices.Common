/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Models;

namespace MicroService.Common.Interfaces
{
    #region IModelCount
    /// <summary>
    /// Represents an object which offers a number of models it holds.
    /// </summary>
    public interface IModelCount
    {
        /// <summary>
        /// Gets the number of models this object currently holds.
        /// </summary>
        /// <returns></returns>
        int GetModelCount();
    }
    #endregion

    #region IUpdatable<T>
    /// <summary>
    /// This object represents a model which can be updated from the given value store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUpdatable<T>
    {
        /// <summary>
        /// Handles value copy operation of a given property.
        /// </summary>
        /// <param name="store">Value store to copy value/s from.</param>
        /// <param name="notification">Out notification to indicate status of copy operation.</param>
        /// <param name="message">Out message to indicate success/failure of copy operation status.</param>
        void Update(IValueStore<T> store, out BindingResultStatus notification, out string message);
    }
    #endregion

    #region IMatch
    /// <summary>
    /// Reprents an object which checks if certqain value exists.
    /// </summary>
    public interface IMatch
    {
        /// <summary>
        /// Finds whether the given value matches the current value of property found by specified property name.
        /// </summary>
        /// <param name="propertyName">NAme of property to search for.</param>
        /// <param name="value">Value to find if there is a match or not.</param>
        /// <returns>True if values match, otherwise false.</returns>
        bool IsMatch(string propertyName, object value);
    }
    #endregion

    #region IModifiable
    //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that is aware of changes in its internal state and allows saving those changes.
    /// </summary>
    public interface IModifiable
    {
        /// <summary>
        /// Saves changes made to this object.
        /// </summary>
        /// <returns>True if the operation is successful; otherwise, false.</returns>
        Task<bool> SaveChanges();
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IExCopyable
    /// <summary>
    /// This interface represents an object that copies data from another model.
    /// </summary>
    internal interface IExCopyable
    {
        /// <summary>
        /// Copies model data from the given model parameter.
        /// </summary>
        /// <param name="model">Model to copy data from.</param>
        /// <returns>True if the copy operation is successful; otherwise, false.</returns>
        Task<bool> CopyFrom(IModel model);
    }
    #endregion

    #region IExModelToDTO
    //-:cnd:noEmit
#if MODEL_USEDTO
    /// <summary>
    /// This interface represents an object which offers a DTO conversion of itself.
    /// </summary>
    internal interface IExModelToDTO 
    {
        /// <summary>
        /// Provides compitible DTO of given type from this model.
        /// You must implement this method to support dtos.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Compitible DTO.</returns>
        IModel ToDTO(Type type);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IFirstModel<TModel, TIDType> 
    /// <summary>
    /// Represents an object which offers the first model with in its internal collection.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TIDType"></typeparam>
    public interface IFirstModel<TModel, TIDType>
        where TModel : Model<TIDType>
        where TIDType : struct
    {
        TModel? GetFirstModel();
    }
    #endregion

    #region IReadable<TModelInterface, TModel, TIDType>
    //-:cnd:noEmit
#if !MODEL_NONREADABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IReadable<TModelInterface, TModel, TIDType>
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
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns></returns>
        Task<TModelInterface> Get(TIDType id);

        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        Task<IEnumerable<TModelInterface>> GetAll(int limitOfResult = 0);

        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        Task<IEnumerable<TModelInterface>> GetAll(int startIndex, int limitOfResult);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IDeletable<TModelInterface, TModel, TIDType>
    //-:cnd:noEmit
#if MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows deleting a single model with a specified ID.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IDeleteable<TModelInterface, TModel, TIDType> 
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
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns></returns>
        Task<TModelInterface> Delete(TIDType id);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IAppendable<TModelInterface, TModel, TIDType>
    //-:cnd:noEmit
#if MODEL_APPENDABLE
    /// <summary>
    /// This interface represents an object that has a list of models to which a new model can be appended.
    /// Any object that implements the IModel interface can be provided. This allows DTOs to be used instead of an actual model object.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IAppendable<TModelInterface, TModel, TIDType> 
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
        /// <summary>
        /// Adds a new model based on the given model.
        /// If the given model is not TModel, then a new appropriate model will be created by copying data from the given model.
        /// </summary>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>Model that is added.</returns>
        Task<TModelInterface> Add(IModel model);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IUpdatable<TModelInterface, TModel, TIDType>
    //-:cnd:noEmit
#if MODEL_UPDATABLE
    /// <summary>
    /// This interface represents an object that has a list of models and allows a model with a specified ID to be updated with data from the given model parameter.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IUpdateable<TModelInterface, TModel, TIDType> 
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
        /// <summary>
        /// Updates a model specified by the given ID with the data of the given model.
        /// </summary>
        /// <param name="id">ID of the model to be updated.</param>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns></returns>
        Task<TModelInterface> Update(TIDType id, IModel model);
    }
#endif
    //+:cnd:noEmit
    #endregion
}
