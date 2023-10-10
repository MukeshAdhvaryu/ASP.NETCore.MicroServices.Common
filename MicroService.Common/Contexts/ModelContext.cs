/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) ||(!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using MicroService.Common.CQRS;
using MicroService.Common.Sets;
#endif
//+:cnd:noEmit

using MicroService.Common.Models;

namespace MicroService.Common.Contexts
{
    #region ModelContext
    /// <summary>
    /// Represents an object which creates list based model context useful for TDD.
    /// </summary>
    public sealed class ModelContext : IModelContext
    {
        #region CREATE COMMAND
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        /// <summary>
        /// Creates new instance of ModelSet<TModel, TID>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        /// <returns>An instance of ModelSet<TModel, TID></returns>
        ICommand<TID, TModel> IModelContext.CreateCommand<TID, TModel>() 
        {
            return new ModelList<TID, TModel>();
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region CREATE QUERY
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IQuery<TModel> IModelContext.CreateQuery<TModel>()
        {
            var list = new List<TModel>();
            return new QuerySet<TModel, List<TModel>>(list, list.AddRange);
        }
#endif
        //+:cnd:noEmit
        #endregion

        public void Dispose() { }

        #region ModelCollection<TModel>
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        /// <summary>
        /// Represents an object which holds a collection of models useful for TDD..
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class ModelList<TID, TModel> : ModelSet<TID, TModel, List<TModel>> 
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
#endif
        //+:cnd:noEmit
        #endregion
    }
#endregion
}
