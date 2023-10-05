/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Sets;

namespace MicroService.Common.Collections
{
    #region IModelContext
    public interface IModelContext : IDisposable
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        , IModifiable
#endif
    //+:cnd:noEmit
    {
        /// <summary>
        /// Creates new instance of ModelSet<TModel, TID>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        /// <returns>An instance of ModelSet<TModel, TID></returns>
        IModelSet<TID, TModel> Create<TID, TModel>()
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct;
    }
    #endregion

    #region ModelContext
    /// <summary>
    /// Represents an object which creates list based model context useful for TDD.
    /// </summary>
    public sealed class ModelContext : IModelContext
    {
        public IModelSet<TID, TModel> Create<TID, TModel>()
        where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct
        {
            return new ModelList<TID, TModel>();
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
        class ModelList<TID, TModel> : ModelSet<TID, TModel, List<TModel>>, IExModelSet<TID, TModel>
            #region TYPE CONSTRAINTS
            where TModel : ISelfModel<TID, TModel>, new()
            where TID : struct
            #endregion
        {
            public ModelList() :
                base(new List<TModel>())
            { }

            #region ADD/ADD RANGE
            //-:cnd:noEmit
#if MODEL_APPENDABLE
            protected override void AddModel(TModel model) =>
                Items.Add(model);
            protected override void AddModels(IEnumerable<TModel> models) =>
                Items.AddRange(models);
#endif
            //+:cnd:noEmit
            #endregion

            #region DELETE/ DELETE RANGE
            //-:cnd:noEmit
#if MODEL_DELETABLE
            public override Task<bool> DeleteRange(IEnumerable<TModel> models)
            {
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
    }
    #endregion
}
