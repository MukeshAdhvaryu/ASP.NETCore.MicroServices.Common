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

    #region ModelCollection<TModel, TID>
    /// <summary>
    /// Represents an object which holds a collection of models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    /// <typeparam name="TID">Type of TID</typeparam>
    public class ModelCollection<TModel, TID> : IModelCollection<TModel, TID>
    //-:cnd:noEmit
#if !MODEL_NONREADABLE
    , IEnumerable<TModel>
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        , IModifiable
#endif
    //+:cnd:noEmit
        #region TYPE CONSTRAINTS
    where TModel : Model<TID>, new()
    where TID : struct
        #endregion
    {
        #region VARIABLES
        List<TModel> models = new List<TModel>(5);
        #endregion

        #region CONSTRUCTORS
        public ModelCollection()
        {
            bool provideSeedData = false;
            var attribute = typeof(TModel).GetCustomAttribute<ModelAttribute>();
            if (attribute != null)
                provideSeedData = attribute.ProvideSeedData;
            if (provideSeedData && !models.Any())
            {
                var model = (IExModel)new TModel();
                var items = model.GetInitialData();
                if (items != null)
                {
                    models.AddRange(items.OfType<TModel>());
                }
            }
        }
        #endregion

        #region FIND
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Finds a model based on given keys.
        /// </summary>
        /// <param name="keys">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public Task<TModel?> Find(IEnumerable<ISearchParameter> keys)
        {
            Predicate<TModel> predicate = (m) =>
            {
                IMatch match = m;

                foreach (var key in keys)
                {
                    if (key == null)
                        continue;
                    if (!match.IsMatch(key))
                        return false;
                }
                return true;
            };
            return Task.FromResult(models.FirstOrDefault((m) => predicate(m)));
        }

        /// <summary>
        /// Finds all models matched based on given key.
        /// </summary>
        /// <param name="key">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public Task<IEnumerable<TModel>> FindAll(ISearchParameter key)
        {
            Func<TModel, bool> func = (m) =>
            {
                return ((IMatch)m).IsMatch(key);
            };
            return Task.FromResult(models.Where(m => func(m)));
        }

        /// <summary>
        /// Finds a model based on given keys.
        /// </summary>
        /// <param name="keys">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public Task<TModel?> Find(TID id)
        {
            return Task.FromResult(models.FirstOrDefault(m => Equals(m.ID, id)));
        }
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
        public Task<bool> Add(TModel model)
        {
            if (models.Any(m => Equals(m.ID, model.ID)))
                return Task.FromResult(false);

            if (Equals(model.ID, default(TID)))
            {
                var exModel = (IExModel<TID>)model;
                exModel.ID = exModel.GetNewID();
            }

            models.Add(model);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Adds a range of specified models.
        /// </summary>
        /// <param name="models">Models to add.</param>
        /// <returns>Task with result of type boolean.</returns>
        public Task<bool> AddRange(IEnumerable<TModel> models)
        {
            var list = new List<TModel>();

            foreach(var model in models)
            {
                if (model == null)
                    continue;
                if (models.Any(m => Equals(m.ID, model.ID)))
                    continue;
                if (Equals(model.ID, default(TID)))
                {
                    var exModel = (IExModel<TID>)model;
                    exModel.ID = exModel.GetNewID();
                }
                list.Add(model);
            }
            this.models.AddRange(list);
            return Task.FromResult(list.Count > 0);
        }
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
        public Task<bool> Delete(TModel model)
        {
            return Task.FromResult(models.Remove(model));
        }

        /// <summary>
        /// Deletes a range of specified models.
        /// </summary>
        /// <param name="models">Models to delete.</param>
        /// <returns>Task with result of type boolean.</returns>
        public Task<bool> DeleteRange(IEnumerable<TModel> models)
        {
            var list = this.models.Except(models).ToList();
            var changed = list.Count != this.models.Count;
            this.models = list;
            return Task.FromResult(changed);
        }
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
        public async Task<bool> Update(TModel model)
        {
            var result = models.FirstOrDefault(m => Equals(m.ID, model.ID));
            if(result == default(TModel))
                return false;
            return await ((IExCopyable)result).CopyFrom(model);
        }

        /// <summary>
        /// Updates a range of specified models.
        /// </summary>
        /// <param name="models">Models to update.</param>
        /// <returns>Task with result of type boolean.</returns>
        public async Task<bool> UpdateRange(IEnumerable<TModel> models)
        {
            var results = models.Select((ext) => Tuple.Create(ext, this.models.FirstOrDefault(m => Equals(m.ID, ext.ID))));
            bool result = false;

            foreach (var tuple in results)
            {
                if (tuple.Item2 == default(TModel))
                    continue;
                var rslt = await ((IExCopyable)tuple.Item2).CopyFrom(tuple.Item1);
                if (rslt)
                    result = true;
            }
            return result;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region SAVE CHANGES
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        Task<bool> IModifiable.SaveChanges() =>
            Task.FromResult(true);
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() =>
            models.FirstOrDefault();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            models.Count();
        #endregion

        #region ENUMERATORS
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        public IEnumerator<TModel> GetEnumerator() =>
            models.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable<TModel>)this).GetEnumerator();
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
