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
    }
    #endregion

    #region IExModel
    /// <summary>
    /// This interface represents a model with primary key named as ID.
    /// </summary>
    internal partial interface IExModel : IModel, IExCopyable, IUpdatable<string>
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
        IReadOnlyList<string> Properties { get; }

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

        /// <summary>
        /// Provides a list of names of properties - must be handled while copying from data supplied from model binder's BindModelAsync method.
        /// If the list is not provided, System.Reflecteion will be used to obtain names of the properties defined in this model.
        /// </summary>
        protected virtual IReadOnlyList<string> Properties => null;

        IReadOnlyList<string> IExModel.Properties
        {
            get
            {
                var propertyNames = this.Properties;

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
        }
        #endregion

        #region UPDATE
        /// <summary>
        /// Handles value copy operation of a given property.
        /// </summary>
        /// <param name="store">Value store to copy value/s from.</param>
        /// <param name="notification">Out notification to indicate status of copy operation.</param>
        /// <param name="message">Out message to indicate success/failure of copy operation status.</param>
        protected abstract void Update(IValueStore<string> value, out BindingResultStatus notification, out string message);

        void IUpdatable<string>.Update(IValueStore<string> store, out BindingResultStatus notification, out string message) =>
            Update(store, out notification, out message);
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

        IEnumerable<IModel> IExModel.GetInitialData() => GetInitialData();
        #endregion

        #region GET NEW ID
        protected abstract TIDType GetNewID();
        TIDType IExModel<TIDType>.GetNewID()
        {
            return GetNewID();
        }
        #endregion

        #region MATCH
        protected abstract bool Match(string propertyName, object value);
        bool IMatch.IsMatch(string propertyName, object value)
        {
            if (propertyName == nameof(ID))
            {
                if (value is TIDType)
                    return Equals(id, value);
                return false;
            }
            return Match(propertyName, value);
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