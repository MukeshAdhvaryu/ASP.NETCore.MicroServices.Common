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

        /// <summary>
        /// Gets ID of a model created last.
        /// </summary>
        /// <returns></returns>
        TIDType GetLastGeneratedID();
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

        /// <summary>
        /// Gets an action for a specified OptionBuilder.
        /// </summary>
        /// <typeparam name="TOptionBuilder">Type of OptionBuilder</typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        Action<TOptionBuilder> GetOptionsBuilderAction<TOptionBuilder>(params IParameter[] parameters);
    }
    #endregion
}