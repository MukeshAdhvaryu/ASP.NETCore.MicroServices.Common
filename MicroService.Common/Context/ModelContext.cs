/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.CQRS;
using MicroService.Common.Models;
using MicroService.Common.Sets;

namespace MicroService.Common.Collections
{
    #region IModelContext
    public interface IModelContext : IDisposable
    {
        /// <summary>
        /// Creates new instance of ModelSet<TModel, TID>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        /// <returns>An instance of ModelSet<TModel, TID></returns>
        IModels<TID, TModel> Create<TID, TModel>()
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct;

        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IModelQuery<TModel> Create<TModel>()
            where TModel : class, ISelfModel<TModel>, new();

        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IModelQuery<TModel> Create<TModel, TItems>(TItems items)
            where TModel : class, ISelfModel<TModel>, new()
            where TItems : IEnumerable<TModel>;
#endif
        //+:cnd:noEmit

    }
    #endregion

    #region ModelContext
    /// <summary>
    /// Represents an object which creates list based model context useful for TDD.
    /// </summary>
    public sealed class ModelContext : IModelContext
    {
        #region CREATE
        IModels<TID, TModel> IModelContext.Create<TID, TModel>()
        {
            return new ModelList<TID, TModel>();
        }
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        IModelQuery<TModel> IModelContext.Create<TModel>()
        {
            return new QueryList<TModel, List<TModel>>(new List<TModel>());
        }

        IModelQuery<TModel> IModelContext.Create<TModel, TItems>(TItems items)
        {
            return new QueryList<TModel, TItems>(items);
        }
#endif
        //+:cnd:noEmit
        #endregion

        public void Dispose() { }

        #region Child Classes
        #region ModelCollection<TModel>
        /// <summary>
        /// Represents an object which holds a collection of models useful for TDD..
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class ModelList<TID, TModel> : Models<TID, TModel, List<TModel>>, IExModels<TID, TModel>
            #region TYPE CONSTRAINTS
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct
            #endregion
        {
            public ModelList() :
                base(new List<TModel>())
            { }

            #region ADD MODELS
            protected override void AddModels(IEnumerable<TModel> items) =>
                Items.AddRange(items);
            #endregion

            #region ADD/ADD RANGE
            //-:cnd:noEmit
#if MODEL_APPENDABLE
            protected override void AddModel(TModel model) =>
                Items.Add(model);
#endif
            //+:cnd:noEmit
            #endregion

            #region DELETE/ DELETE RANGE
            //-:cnd:noEmit
#if MODEL_DELETABLE
            public override Task<bool> DeleteRange(IEnumerable<TModel>? models)
            {
                if (models == null)
                    return Task.FromResult(false);

                var list = models.Except(models).ToList();
                Items.Clear();
                Items.AddRange(list);
                var changed = list.Count != 0;
                return Task.FromResult(changed);
            }

            protected override bool RemoveModel(TModel model) =>
               Items.Remove(model);
#endif
            //+:cnd:noEmit
            #endregion
        }
        #endregion

        #region QueryList<TModel, TItems>
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Represents an object which holds a collection of models useful for TDD..
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class QueryList<TModel, TItems> : QueryModels<TModel, TItems>
            #region TYPE CONSTRAINTS
            where TModel : ISelfModel<TModel>, new()
            where TItems : IEnumerable<TModel>
            #endregion
        {
            public QueryList(TItems items) :
                base(items, false)
            { }

            #region ADD MODELS
            protected override void AddModels(IEnumerable<TModel> items) { }
            #endregion
        }
#endif
        //+:cnd:noEmit
        #endregion
        #endregion
    }
    #endregion
}
