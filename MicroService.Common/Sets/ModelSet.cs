/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using System.Reflection;

using MicroService.Common.Attributes;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Sets
{
    #region IModelSet<TModel>
    public interface IModelSet<TModel> : IFirstModel, IModelCount, IFirstModel<TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        #endregion
    { }
    #endregion

    #region IModelSet
    internal interface IExModelSet<TModel> : IModelSet<TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel> 
        #endregion
    {
    }
    #endregion

    public abstract class ModelSet<TModel, TItems> : IExModelSet<TModel> 
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>, new()
        where TItems : IEnumerable<TModel>
        #endregion
    {
        #region VARIABLES
        protected readonly TItems Items;
        #endregion

        #region CONSTRUCTORS
        public ModelSet(TItems models) :
            this(models, true)
        { }
        public ModelSet(TItems models, bool initializeData = true)
        {
            Items = models;
            if (!initializeData)
                return;

            IEnumerable<TModel>? items = null;
            bool provideSeedData = false;
            var attribute = typeof(TModel).GetCustomAttribute<DBConnectAttribute>();
            if (attribute != null)
                provideSeedData = attribute.ProvideSeedData;
            if (provideSeedData && !Items.Any())
            {
                var model = (IExModel)new TModel();
                items = model.GetInitialData()?.OfType<TModel>();
            }
            if (items == null)
                return;
            try
            {
                AddModels(items);
            }
            catch { }
        }
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel>.GetFirstModel() =>
            Items.FirstOrDefault();
        IModel? IFirstModel.GetFirstModel() =>
            Items.FirstOrDefault();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            Items.Count();
        #endregion

        #region ADD MODELS
        /// <summary>
        /// Adds a range of specified models.
        /// </summary>
        /// <param name="models">Models to add.</param>
        /// <returns>Task with result of type boolean.</returns>
        protected abstract void AddModels(IEnumerable<TModel> models);
        #endregion
    }
}
