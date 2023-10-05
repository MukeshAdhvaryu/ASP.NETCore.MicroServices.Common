/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Collections;
using MicroService.Common.CQRS;
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
    #region IModels<TModel, TID>
    /// <summary>
    /// Represents an object which holds a collection of models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    /// <typeparam name="TID">Type of TID</typeparam>
    public partial interface IModels<TID, TModel> : IFirstModel<TModel, TID>
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        , IFindByID<TModel, TModel, TID>
        , IEnumerable<TModel>
        , IModelQuery<TModel>
#endif
#if MODEL_DELETABLE ||MODEL_APPENDABLE ||MODEL_UPDATABLE
        , IModelCommand<TID, TModel>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel>, new()
        where TID : struct
        #endregion
    { }
    #endregion

    #region IExModels<TModel, TID>
    internal partial interface IExModels<TID, TModel> : IExModelSet<TModel>, IModels<TID, TModel>
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        , IExModelQuery<TModel>
#endif
#if MODEL_DELETABLE || MODEL_APPENDABLE || MODEL_UPDATABLE
        , IExModelCommand<TID, TModel>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel> , new()
        where TID : struct
        #endregion
    {
    }
    #endregion

    #region ModelCommand<TID, TModel>
    /// <summary>
    /// Represents an object which holds a collection of models useful for TDD..
    /// </summary>
    /// <typeparam name="TID">Type of TID</typeparam>
    /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
    public abstract class Models<TID, TModel, TItems> : ModelSet<TModel, TItems>, IExModels<TID, TModel> 
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel>, new()
        where TID : struct
        where TItems : IEnumerable<TModel>
        #endregion
    {
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        IModelQuery<TModel> Query;
#endif
        //+:cnd:noEmit

        #region CONSTRUCTORS
        public Models(TItems models) :
            base(models)
        {
            //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
            Query = GetQueryObject(models);
#endif
            //+:cnd:noEmit

        }
        #endregion

        #region GET QUERY OBJECT
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        protected virtual IModelQuery<TModel> GetQueryObject(TItems models)
        {
            using (IModelContext context = new ModelContext())
            {
                return context.Create<TModel, TItems>(models);
            }
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region IsFOUND
        protected virtual bool IsFound(TModel model, out TModel? result, bool addOperation = false)
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
        bool IExModelCommand<TID, TModel>.IsFound(TModel model, out TModel? result, bool addOperation) =>
            IsFound(model, out result, addOperation);
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() =>
            Items.FirstOrDefault();
        #endregion

        #region GET (id)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
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
            if (model == null)
                return Task.FromResult(false);

            if (IsFound(model, true))
                return Task.FromResult(false);

            AddModel(model);
            return Task.FromResult(true);
        }
        protected abstract void AddModel(TModel model);
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
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public Task<IEnumerable<TModel>?> GetAll(int count = 0) =>
            Query.GetAll(count);
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (startIndex, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public Task<IEnumerable<TModel>?> GetAll(int startIndex, int count) =>
            Query.GetAll(startIndex, count);
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameters, conditionJoin)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Finds a model based on given paramters.
        /// </summary>
        /// <param name="paramters">Parameters to be used to find the model.</param>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        public Task<TModel?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin) =>
            Query.Find(parameters, conditionJoin);
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters, conditionJoin)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        public Task<IEnumerable<TModel>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin) =>
            Query.FindAll(parameters, conditionJoin);
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters, conditionJoin)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Finds all models matched based on given parameter.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        public Task<IEnumerable<TModel>?> FindAll(ISearchParameter? parameter) =>
            Query.FindAll(parameter);
#endif
        //+:cnd:noEmit
        #endregion

        #region ENUMERATORS
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        public IEnumerator<TModel> GetEnumerator() =>
            Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
