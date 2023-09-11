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

}
