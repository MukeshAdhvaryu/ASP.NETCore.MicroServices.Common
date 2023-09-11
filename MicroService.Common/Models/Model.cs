/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using MicroService.Common.Interfaces;

namespace MicroService.Common.Models
{
    #region IModel
    /// <summary>
    /// This interface represents a model.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    public interface IModel
    { }
    #endregion

    #region IModel<TIDType>
    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    /// <typeparam name="TIDType"></typeparam>
    public interface IModel<TIDType> : IModel, IMatch
        where TIDType : struct
    {
        /// <summary>
        /// gets primary key value of this model.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        TIDType ID { get; }
    }
    #endregion

    #region IExModel<TIDType>
    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    /// <typeparam name="TIDType"></typeparam>
    internal interface IExModel<TIDType> : IModel<TIDType>, IExModel
        where TIDType : struct
    {
        /// <summary>
        /// gets primary key value of this model.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        new TIDType ID { get; set; }

        /// <summary>
        /// Gets unique id.
        /// </summary>
        /// <returns>Newly generated id.</returns>
        TIDType GetNewID();

        /// <summary>
        /// Tries to parse the given value to the type of ID
        /// Returns parsed value if succesful, otherwise default value.
        /// </summary>
        /// <param name="value">Value to be parsed as TIDType.</param>
        /// <param name="newID">Parsed value.</param>
        /// <returns>True if succesful, otherwise false</returns>
        bool TryParseID(object value, out TIDType newID);
    }
    #endregion

    #region IExModel
    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// </summary>
    internal partial interface IExModel : IModel, IExCopyable, IExParamParser
    //-:cnd:noEmit
#if MODEL_USEDTO
        , IExModelToDTO
#endif
    //+:cnd:noEmit
    {
        /// <summary>
        /// Provides a list of names of properties - must be handled while copying from data supplied from model binder's BindModelAsync method.
        /// If the list is not provided, System.Reflecteion will be used to obtain names of the properties defined in this model.
        /// </summary>
        IReadOnlyList<string> GetPropertyNames(bool forSearch = false);

        /// <summary>
        /// Gets initial data.
        /// </summary>
        /// <returns>IEnumerable\<IModel\> containing list of initial data.</returns>
        IEnumerable<IModel> GetInitialData();
    }
    #endregion

    #region Model<TIDType>
    /// <summary>
    /// This class represents a base class for any user-defined model.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    public abstract partial class Model<TIDType> : IExModel<TIDType>, IExModel
        where TIDType : struct
    {
        #region VARIABLES
        protected TIDType id;
        #endregion

        #region CONSTRUCTOR
        protected Model(bool generateNewID)
        {
            if (generateNewID)
                id = GetNewID();
        }
        #endregion

        #region PROPERTIES
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TIDType ID { get => id; set => id = value; }
        TIDType IExModel<TIDType>.ID { get => id; set => id = value; }
        #endregion

        #region GET PROPERTY NAMES
        /// <summary>
        /// Provides a list of names of properties - must be handled while copying from data supplied from model binder's BindModelAsync method.
        /// If the list is not provided, System.Reflecteion will be used to obtain names of the properties defined in this model.
        /// </summary>
        protected virtual IReadOnlyList<string> GetPropertyNames(bool forSearch = false) => null;

        IReadOnlyList<string> IExModel.GetPropertyNames(bool forSearch)
        {
            var propertyNames = this.GetPropertyNames(forSearch);

            if (propertyNames == null || propertyNames.Count == 0)
            {
                propertyNames = GetType().GetProperties().Where(p =>
                {
                    var attr = p.GetType().GetCustomAttribute<DatabaseGeneratedAttribute>();
                    if (attr?.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed)
                        return false;
                    return true;
                }).Select(p => p.Name).ToArray();
            }
            return propertyNames;
        }
        #endregion

        #region PARSE
        /// <summary>
        /// Parses the specified parameter and if possible emits the value compitible with
        /// the property this object posseses.
        /// </summary>
        /// <param name="parameter">Parameter to parse.</param>
        /// <param name="currentValue">Current value exists for the given property in this object.</param>
        /// <param name="parsedValue">If succesful, a compitible value parsed using supplied value from parameter.</param>
        /// <param name="updateValueIfParsed">If succesful, replace the current value with the compitible parsed value.</param>
        /// <returns>Result Message with Status of the parse operation.</returns>
        protected abstract Message Parse(IParameter parameter, out object currentValue, out object parsedValue, bool updateValueIfParsed = false);

        Message IExParamParser.Parse(IParameter parameter, out object currentValue, out object parsedValue, bool updateValueIfParsed)
        {
            var name = parameter.Name;

            switch (name)
            {
                case nameof(ID):
                    currentValue = id;
                    var value = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
                    parsedValue = null;
                    if(((IExModel<TIDType>)this).TryParseID(value, out TIDType newID))
                    {
                        parsedValue = newID;
                        if(updateValueIfParsed &&  Equals(id , default(TIDType)))
                            id = newID;
                        
                        return Message.Sucess(name);
                    }
                    return Message.Ignored(name);
                default:
                    break;
            }
            return Parse(parameter, out currentValue, out parsedValue, updateValueIfParsed);
        }
        #endregion

        #region COPY FROM
        /// <summary>
        /// Copies data from the given model to this instance.
        /// </summary>
        /// <param name="model">Model to copy data from.</param>
        /// <returns></returns>
        protected abstract Task<bool> CopyFrom(IModel model);

        Task<bool> IExCopyable.CopyFrom(IModel model)
        {
            if (model is IModel<TIDType>)
            {
                var m = (IModel<TIDType>)model;
                if (Equals(id, default(TIDType)))
                    id = m.ID;
                return CopyFrom(model);
            }

            //-:cnd:noEmit
#if MODEL_USEDTO
            if (model is IModel)
            {
                if (Equals(id, default(TIDType)))
                    id = GetNewID();
                return CopyFrom(model);
            }
#endif
            //+:cnd:noEmit
            return Task.FromResult(false);
        }
        #endregion

        #region GET INITIAL DATA
        /// <summary>
        /// Gets initial data.
        /// </summary>
        /// <returns>IEnumerable\<IModel\> containing list of initial data.</returns>
        protected abstract IEnumerable<IModel> GetInitialData();

        IEnumerable<IModel> IExModel.GetInitialData() =>
            GetInitialData();
        #endregion

        #region GET NEW ID
        protected abstract TIDType GetNewID();
        TIDType IExModel<TIDType>.GetNewID()
        {
            return GetNewID();
        }
        #endregion

        #region TRY PARSE ID
        /// <summary>
        /// Tries to parse the given value to the type of ID
        /// Returns parsed value if succesful, otherwise default value.
        /// </summary>
        /// <param name="value">Value to be parsed as TIDType.</param>
        /// <param name="newID">Parsed value.</param>
        /// <returns>True if succesful, otherwise false</returns>
        protected abstract bool TryParseID(object value, out TIDType newID);

        bool IExModel<TIDType>.TryParseID(object value, out TIDType newID)
        {
            if(value == null)
            {
                newID = default(TIDType);
                return false;
            }
            if (value is TIDType)
            {
                newID = (TIDType)value;
                return true;
            }
            return TryParseID(value, out newID);
        }
        #endregion

        #region IsMATCH
        /// <summary>
        /// Matches property with specified name using criteria given and emits current value exists for the given property
        /// and a compitible value parsed using supplied value from parameter.
        /// </summary>
        /// <param name="propertyName">Name of property which to match for.</param>
        /// <param name="criteria">Criteria enum to specify on the grounds the match should perform.</param>
        /// <param name="currentValue">Current value exists for the given property.</param>
        /// <param name="parsedValue">If succesful, a compitible value parsed using supplied value from parameter.</param>
        /// <returns>True if values match, otherwise false.</returns>
        protected virtual bool IsMatch (string propertyName, Criteria criteria, object currentValue, object parsedValue)
        {
            return Operations.Compare(currentValue, criteria, parsedValue);
        }
        bool IMatch.IsMatch(ISearchParameter parameter)
        {
            var result = Parse(parameter, out var currentValue, out object newValue);
            switch (result.Status)
            {
                case ResultStatus.Sucess:
                    return IsMatch(parameter.Name, parameter.Criteria, currentValue, newValue);
                case ResultStatus.Failure:
                case ResultStatus.Ignored:
                case ResultStatus.MissingValue:
                case ResultStatus.MissingRequiredValue:
                default:
                    return false;
            }
        }
        #endregion

        //-:cnd:noEmit
#if MODEL_USEDTO
        #region IModelToDTO
        /// <summary>
        /// Provides compitible DTO of given type from this model.
        /// You must override this method to support dtos.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Compitible DTO.</returns>
        protected virtual IModel ToDTO(Type type)
        {
            var t = GetType();
            if(type == t || t.IsAssignableTo(type))
                return this;
            return null;
        }
        IModel IExModelToDTO.ToDTO(Type type) =>
            ToDTO(type);
        #endregion
#endif
        //+:cnd:noEmit
    }
    #endregion
}