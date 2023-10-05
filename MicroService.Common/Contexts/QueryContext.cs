/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !MODEL_NONQUERYABLE
//+:cnd:noEmit

using MicroService.Common.Models;

namespace MicroService.Common.Collections
{
    #region IQueryContext
    public interface IQueryContext : IDisposable
    {
        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IQueryModels<TModel> Create<TModel>()
            where TModel : ISelfModel<TModel>, new();

        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IQueryModels<TModel> Create<TModel, TItems>(TItems items)
            where TModel : ISelfModel<TModel>, new()
            where TItems : IEnumerable<TModel>;
    }
    #endregion

    #region QueryContext
    /// <summary>
    /// Represents an object which creates list based model query context useful for TDD.
    /// </summary>
    public sealed class ModelQueryContext : IQueryContext
    {
        public IQueryModels<TModel> Create<TModel>()
            where TModel : ISelfModel<TModel>, new()
        {
            return new ModelList<TModel>();
        }

        public IQueryModels<TModel> Create<TModel, TItems>(TItems items)
            where TModel : ISelfModel<TModel>, new()
            where TItems : IEnumerable<TModel>
        {
            return new ModelList<TModel, TItems>(items);
        }

        public void Dispose() { }

        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        public Task<bool> SaveChanges() =>
            Task.FromResult(true);
#endif
        //+:cnd:noEmit

        #region ModelCollection<TModel>
        /// <summary>
        /// Represents an object which holds a collection of models useful for TDD..
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class ModelList<TModel, TItems> : QueryModels<TModel, TItems>
            #region TYPE CONSTRAINTS
            where TModel : ISelfModel<TModel>, new()
            where TItems : IEnumerable<TModel>
            #endregion
        {
            public ModelList(TItems items) :
                base(items, false)
            { }

            #region ADD MODELS
            protected override void AddModels(IEnumerable<TModel> items) { }
            #endregion
        }
        #endregion

        #region ModelCollection<TModel>
        /// <summary>
        /// Represents an object which holds a collection of models useful for TDD..
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class ModelList<TModel> : QueryModels<TModel, List<TModel>>
            #region TYPE CONSTRAINTS
            where TModel : ISelfModel<TModel>, new()
            #endregion
        {
            public ModelList() :
                base(new List<TModel>())
            { }

            #region ADD MODELS
            protected override void AddModels(IEnumerable<TModel> items) =>
                Items.AddRange(items);
            #endregion
        }
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
