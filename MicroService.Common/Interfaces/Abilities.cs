/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Exceptions;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

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

    #region IMatch
    /// <summary>
    /// Reprents an object which checks if certqain value exists.
    /// </summary>
    public interface IMatch
    {
        /// <summary>
        /// Finds whether the given value matches the current value of property found by specified property name.
        /// </summary>
        /// <param name="searchParameter">Search parameter to use to match records in this object.</param>
        /// <returns>True if values match, otherwise false.</returns>
        bool IsMatch(ISearchParameter searchParameter);
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

    #region IExParamParser
    /// <summary>
    /// This interface represents an object that offers parameter parsing capability.
    /// Provided, the given property exist as one of its members.
    /// </summary>
    internal interface IExParamParser
    {
        /// <summary>
        /// Parses the specified parameter and if possible emits the value compitible with
        /// the property this object posseses.
        /// </summary>
        /// <param name="parameter">Parameter to parse.</param>
        /// <param name="currentValue">Current value exists for the given property in this object.</param>
        /// <param name="parsedValue">If succesful, a compitible value parsed using supplied value from parameter.</param>
        /// <param name="updateValueIfParsed">If succesful, replace the current value with the compitible parsed value.</param>
        /// <returns>Result Message with Status of the parse operation.</returns>
        /// <param name="criteria">Criteria to be used when parsing value.</param>
        Message Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed = false, Criteria criteria = 0);
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
        IModel? ToDTO(Type type);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IFirstModel<TModel, TID> 
    /// <summary>
    /// Represents an object which offers the first model with in its internal collection.
    /// </summary>
    public interface IFirstModel  
    {
        IModel? GetFirstModel();
    }
    #endregion

    #region IFirstModel<TModel, TID> 
    /// <summary>
    /// Represents an object which offers the first model with in its internal collection.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IFirstModel<TModel, TID> : IFirstModel
        where TModel : Model<TID>
        where TID : struct
    {
        new TModel? GetFirstModel();
    }
    #endregion

    #region IReadable<TOutDTO, TModel, TID>
    //-:cnd:noEmit
#if !MODEL_NONREADABLE
    /// <summary>
    /// This interface represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IReadable<TOutDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns></returns>
        Task<TOutDTO> Get(TID id);

        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        Task<IEnumerable<TOutDTO>> GetAll(int limitOfResult = 0);

        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        Task<IEnumerable<TOutDTO>> GetAll(int startIndex, int limitOfResult);

        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        Task<IEnumerable<TOutDTO>> FindAll(ISearchParameter parameter);

        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        Task<IEnumerable<TOutDTO>> FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin = 0);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IDeletable<TOutDTO, TModel, TID>
    //-:cnd:noEmit
#if MODEL_DELETABLE
    /// <summary>
    /// This interface represents an object that allows deleting a single model with a specified ID.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IDeleteable<TOutDTO, TModel, TID> 
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns></returns>
        Task<TOutDTO> Delete(TID id);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IAppendable<TOutDTO, TModel, TID>
    //-:cnd:noEmit
#if MODEL_APPENDABLE
    /// <summary>
    /// This interface represents an object that has a list of models to which a new model can be appended.
    /// Any object that implements the IModel interface can be provided. This allows DTOs to be used instead of an actual model object.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IAppendable<TOutDTO, TModel, TID> 
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
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
        /// <returns>Model that is added.</returns>
        Task<TOutDTO> Add(IModel model);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IUpdatable<TOutDTO, TModel, TID>
    //-:cnd:noEmit
#if MODEL_UPDATABLE
    /// <summary>
    /// This interface represents an object that has a list of models and allows a model with a specified ID to be updated with data from the given model parameter.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IUpdatable<TOutDTO, TModel, TID> 
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
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
        /// <returns></returns>
        Task<TOutDTO> Update(TID id, IModel model);
    }
#endif
    //+:cnd:noEmit
    #endregion

    #region IExModelExceptionSupplier
    /// <summary>
    /// This interface represents an object which supplies an appropriate exception for a failure in a specified method.
    /// </summary>
    internal interface IExModelExceptionSupplier
    {
        /// <summary>
        /// Supplies an appropriate exception for a failure in a specified method.
        /// </summary>
        /// <param name="exceptionType">Type of exception to get.</param>
        /// <param name="additionalInfo">Additional information to aid the task of exception supply.</param>
        /// <param name="innerException">Inner exception which is already thrown.</param>
        /// <returns>Instance of SpecialException class.</returns>
        ModelException GetModelException(ExceptionType exceptionType, string? additionalInfo = null, Exception? innerException = null);
    }
    #endregion
}
