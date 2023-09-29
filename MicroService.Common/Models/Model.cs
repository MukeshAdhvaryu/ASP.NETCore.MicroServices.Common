/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using MicroService.Common.Interfaces;
using MicroService.Common.Parameters;

namespace MicroService.Common.Models
{
    #region Model<TID>
    /// <summary>
    /// This class represents a base class for any user-defined model.
    /// Highly customizable by using the following conditional compilation symbols:
    /// MODEL_DELETABLE;
    /// MODEL_APPENDABLE;
    /// MODEL_UPDATABLE;
    /// MODEL_USEMYOWNCONTROLLER
    /// </summary>
    public abstract partial class Model<TID> : IExModel<TID>, IExModel
        where TID : struct
    {
        #region VARIABLES
        protected TID id;
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
        public TID ID { get => id; set => id = value; }
        TID IExModel<TID>.ID { get => id; set => id = value; }
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
        protected abstract Message Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed = false);

        Message IExParamParser.Parse(IParameter parameter, out object? currentValue, out object? parsedValue, bool updateValueIfParsed, Criteria criteria)
        {
            var name = parameter.Name;
            parsedValue = null;
            object? value;

            switch (name)
            {
                case nameof(ID):
                    currentValue = id;
                    value = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
                    switch (criteria)
                    {
                        case Criteria.Occurs:
                        case Criteria.BeginsWith:
                        case Criteria.EndsWith:
                        case Criteria.OccursNoCase:
                        case Criteria.BeginsWithNoCase:
                        case Criteria.EndsWithNoCase:
                        case Criteria.StringEqual:
                        case Criteria.StringEqualNoCase:
                        case Criteria.StringNumGreaterThan:
                        case Criteria.StringNumLessThan:
                        case Criteria.NotOccurs:
                        case Criteria.NotBeginsWith:
                        case Criteria.NotEndsWith:
                        case Criteria.NotOccursNoCase:
                        case Criteria.NotBeginsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                        case Criteria.NotStrEqual:
                        case Criteria.NotStrEqualNoCase:
                        case Criteria.NotStringGreaterThan:
                        case Criteria.NotStringLessThan:
                            parsedValue = value.ToString();
                            return Message.Sucess(name);
                        default:
                            break;
                    }
                    if(((IExModel<TID>)this).TryParseID(value, out TID newID))
                    {
                        parsedValue = newID;
                        if(updateValueIfParsed &&  Equals(id , default(TID)))
                            id = newID;
                        
                        return Message.Sucess(name);
                    }
                    return Message.Ignored(name);
                default:
                    switch (criteria)
                    {
                        case Criteria.Occurs:
                        case Criteria.BeginsWith:
                        case Criteria.EndsWith:
                        case Criteria.OccursNoCase:
                        case Criteria.BeginsWithNoCase:
                        case Criteria.EndsWithNoCase:
                        case Criteria.StringEqual:
                        case Criteria.StringEqualNoCase:
                        case Criteria.StringNumGreaterThan:
                        case Criteria.StringNumLessThan:
                        case Criteria.NotOccurs:
                        case Criteria.NotBeginsWith:
                        case Criteria.NotEndsWith:
                        case Criteria.NotOccursNoCase:
                        case Criteria.NotBeginsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                        case Criteria.NotStrEqual:
                        case Criteria.NotStrEqualNoCase:
                        case Criteria.NotStringGreaterThan:
                        case Criteria.NotStringLessThan:
                            value = parameter is IModelParameter ? ((IModelParameter)parameter).FirstValue : parameter.Value;
                            parsedValue = value.ToString();
                            currentValue = value;
                            return Message.Sucess(name);
                        default:
                            break;
                    }
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
            if (model is IModel<TID>)
            {
                var m = (IModel<TID>)model;
                if (Equals(id, default(TID)))
                    id = m.ID;
                return CopyFrom(model);
            }

            //-:cnd:noEmit
#if MODEL_USEDTO
            if (model is IModel)
            {
                if (Equals(id, default(TID)))
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
        protected abstract TID GetNewID();
        TID IExModel<TID>.GetNewID()
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
        protected abstract bool TryParseID(object value, out TID newID);

        bool IExModel<TID>.TryParseID(object value, out TID newID)
        {
            if(value == null)
            {
                newID = default(TID);
                return false;
            }
            if (value is TID)
            {
                newID = (TID)value;
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