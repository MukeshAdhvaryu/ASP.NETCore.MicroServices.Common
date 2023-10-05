/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Attributes;
using System.Reflection;

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

//-:cnd:noEmit
#if !MODEL_NONREADABLE
using MicroService.Common.Parameters;
using System.Collections;
#endif
//+:cnd:noEmit

namespace MicroService.Common.Sets
{
    #region IModelSet
    public interface IModelSet : IFirstModel, IModelCount
    { }
    #endregion

    #region IModelSet<TModel, TID>
    /// <summary>
    /// Represents an object which holds a collection of models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    /// <typeparam name="TID">Type of TID</typeparam>
    public partial interface IModelSet<TID, TModel> : IModelSet, IFirstModel<TModel, TID>
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        , IReadable<TModel, TModel, TID>
        , IEnumerable<TModel>
#endif
#if MODEL_DELETABLE
  , IDelete<TModel>
#endif
#if MODEL_APPENDABLE
  , IAdd<TModel>
#endif
#if MODEL_UPDATABLE
  , IUpdate<TModel>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TID, TModel>
        where TID : struct
        #endregion
    { }
    #endregion

    #region IExModelSet<TModel, TID>
    /// <summary>
    /// Represents an object which holds a collection of models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    /// <typeparam name="TID">Type of TID</typeparam>
    internal partial interface IExModelSet<TID, TModel> : IModelSet<TID, TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TID, TModel> 
        where TID : struct
        #endregion
    {
        //-:cnd:noEmit
#if MODEL_NONREADABLE
        /// <summary>
        /// Finds a model based on given id.
        /// </summary>
        /// <param name="id">ID to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        Task<TModel?> Get(TID? id);
#endif     
        //+:cnd:noEmit
    }
    #endregion

    #region ModelSet<TID, TModel>
    /// <summary>
    /// Represents an object which holds a collection of models useful for TDD..
    /// </summary>
    /// <typeparam name="TID">Type of TID</typeparam>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    public abstract class ModelSet<TID, TModel, TItems> : IExModelSet<TID, TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TID, TModel>, new()
        where TID : struct
        where TItems : IEnumerable<TModel>
        #endregion
    {
        #region PROPERTIES
        protected readonly TItems Items;
        #endregion

        #region CONSTRUCTORS
        public ModelSet(TItems models)
        {
            Items = models;
            var initialData = GetInitialData();
            if (initialData == null)
                return;
            try
            {
                AddRange(initialData);
            }
            catch { }
        }
        #endregion

        #region GET INITIAL DATA
        protected IEnumerable<TModel>? GetInitialData()
        {
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
            return items;
        }
        #endregion

        #region IsFOUND
        protected bool IsFound(TModel model, out TModel? result, bool addOperation = false)
        {
            result = default(TModel);

            var any = Items.Any(m => Equals(m.ID, model.ID));

            if (addOperation && any)
                return false;

            if (!addOperation && !any)
                return false;

            if (!any && addOperation && Equals(model.ID, default(TID)))
            {
                var exModel = (IExModel<TID>)model;
                exModel.ID = exModel.GetNewID();
            }
            result = model;
            return addOperation && any || !addOperation && any;
        }
        protected bool IsFound(TModel model, bool addOperation = false) =>
            IsFound(model, out _, addOperation);
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() =>
            Items.FirstOrDefault();
        #endregion

        #region GET (id)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Finds a model based on given keys.
        /// </summary>
        /// <param name="keys">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public Task<TModel?> Get(TID? id)
        {
            return Task.FromResult(Items.FirstOrDefault(m => Equals(m.ID, id)));
        }
#else
        Task<TModel?> IExModelSet<TModel, TID>.Get(TID? id) =>
            Task.FromResult(Models.FirstOrDefault(m => Equals(m.ID, id)));
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
        public Task<bool> Add(TModel? model)
        {
            if(model == null)
                return Task.FromResult(false);

            if (IsFound(model, true))
                return Task.FromResult(false);

            AddModel(model);
            return Task.FromResult(true);
        }
        protected abstract void AddModel(TModel? model);
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD RANGE
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        /// <summary>
        /// Adds a range of specified models.
        /// </summary>
        /// <param name="models">Models to add.</param>
        /// <returns>Task with result of type boolean.</returns>
        public Task<bool> AddRange(IEnumerable<TModel>? models)
        {
            if (models == null)
                return Task.FromResult(false);

            var list = models.Where(m => m != null && !IsFound(m, true));
            AddModels(list);
            return Task.FromResult(true);
        }
        protected abstract void AddModels(IEnumerable<TModel> models);
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
        public Task<bool> Delete(TModel? model)
        {
            if (model == null)
                return Task.FromResult(false);
            return Task.FromResult(RemoveModel(model));
        }

        /// <summary>
        /// Deletes a range of specified models.
        /// </summary>
        /// <param name="models">Models to delete.</param>
        /// <returns>Task with result of type boolean.</returns>
        /// 
        public abstract Task<bool> DeleteRange(IEnumerable<TModel>? models);

        protected abstract bool RemoveModel(TModel model);
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
        public async Task<bool> Update(TModel? model)
        {
            if (model == null)
                return (false);
            if (!IsFound(model, out TModel? result) || result == null)
                return false;

            return await ((IExCopyable)result).CopyFrom(model);
        }

        /// <summary>
        /// Updates a range of specified models.
        /// </summary>
        /// <param name="models">Models to update.</param>
        /// <returns>Task with result of type boolean.</returns>
        public async Task<bool> UpdateRange(IEnumerable<TModel>? models)
        {
            if (models == null)
                return (false);
            var results = models.Select(
                m =>
                {
                    if (m != null && IsFound(m, out TModel? item))
                        return Tuple.Create(m, item);
                    return null;
                }
            );
            bool result = false;

            foreach (var tuple in results)
            {
                if (tuple == null || tuple.Item2 == null)
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

        #region GET ALL (count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public Task<IEnumerable<TModel>?> GetAll(int count = 0)
        {
            if (count == 0)
                return Task.FromResult((IEnumerable<TModel>?)Items);

            if (count < 0)
                return Task.FromResult((IEnumerable<TModel>?)Enumerable.Empty<TModel>());

            return Task.FromResult(Items.Take(count) ?? (IEnumerable<TModel>?)Enumerable.Empty<TModel>());
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (startIndex, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public Task<IEnumerable<TModel>?> GetAll(int startIndex, int count)
        {
            if (startIndex > 0)
                startIndex = 0;

            if (count == 0 && startIndex == 0)
                return Task.FromResult((IEnumerable<TModel>?)Items);

            if (count < 0)
                return Task.FromResult((IEnumerable<TModel>?)Enumerable.Empty<TModel>());

            return Task.FromResult((IEnumerable<TModel>?)Items.Skip(startIndex).Take(count));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameters, conditionJoin)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Finds a model based on given paramters.
        /// </summary>
        /// <param name="paramters">Parameters to be used to find the model.</param>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        public Task<TModel?> Find(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin)
        {
            Predicate<TModel> predicate;

            switch (conditionJoin)
            {
                case AndOr.AND:
                default:
                    predicate = (m) =>
                    {
                        IMatch match = m;

                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (!match.IsMatch(key))
                                return false;
                        }
                        return true;
                    };
                    break;
                case AndOr.OR:
                    predicate = (m) =>
                    {
                        IMatch match = m;
                        bool result = false;
                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (match.IsMatch(key))
                            {
                                result = true;
                            }
                        }
                        return result;
                    };
                    break;
            }
            return Task.FromResult(Items.FirstOrDefault((m) => predicate(m)));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        public Task<IEnumerable<TModel>?> FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin)
        {
            Predicate<TModel> predicate;

            switch (conditionJoin)
            {
                case AndOr.AND:
                default:
                    predicate = (m) =>
                    {
                        IMatch match = m;

                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (!match.IsMatch(key))
                                return false;
                        }
                        return true;
                    };
                    break;
                case AndOr.OR:
                    predicate = (m) =>
                    {
                        IMatch match = m;
                        bool result = false;
                        foreach (var key in parameters)
                        {
                            if (key == null)
                                continue;
                            if (match.IsMatch(key))
                            {
                                result = true;
                            }
                        }
                        return result;
                    };
                    break;
            }

            return Task.FromResult((IEnumerable<TModel>?)Items.Where((m) => predicate(m)));
        }

        /// <summary>
        /// Finds all models matched based on given key.
        /// </summary>
        /// <param name="key">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public Task<IEnumerable<TModel>?> FindAll(ISearchParameter key)
        {
            Func<TModel, bool> func = (m) =>
            {
                return ((IMatch)m).IsMatch(key);
            };
            return Task.FromResult((IEnumerable<TModel>?)Items.Where(m => func(m)));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        IModel? IFirstModel.GetFirstModel() =>
            Items.FirstOrDefault();
        TModel? IFirstModel<TModel>.GetFirstModel() =>
            Items.FirstOrDefault();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            Items.Count();
        #endregion

        #region ENUMERATORS
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        public IEnumerator<TModel> GetEnumerator()
        {
            foreach (var item in Items)
                yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
