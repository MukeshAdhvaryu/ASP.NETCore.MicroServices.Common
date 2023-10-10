/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD  
//+:cnd:noEmit

using MicroService.Common.Contexts;
//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) ||(!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using MicroService.Common.CQRS;
using MicroService.Common.Sets;
#endif
//+:cnd:noEmit
using MicroService.Common.Models;

using Microsoft.EntityFrameworkCore;

namespace MicroService.Common.Web.API
{
    #region DBCONTEXT
    public partial class DBContext : DbContext, IModelContext
    {
        #region CONSTRUCTOR
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        { }
        #endregion

        #region ON MODEL CREATION
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly);
        }
        #endregion

        #region CREATE COMMAND
        //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
        /// <summary>
        /// Creates new instance of ModelSet<TModel, TID>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        /// <returns>An instance of ModelSet<TModel, TID></returns>
        ICommand<TID, TModel> IModelContext.CreateCommand<TID, TModel>()
        {
            return new ModelList<TID, TModel>(this, Set<TModel>());
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
            var list = Set<TModel>();
            var set = new QuerySet<TModel, DbSet<TModel>>(list, list.AddRange);
            SaveChanges();
            return set;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ENTITY LIST
        //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
        /// <summary>
        /// Represents an object which holds a collection of models indirectly.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class ModelList<TID, TModel> : ModelSet<TID, TModel, DbSet<TModel>>
            #region TYPE CONSTRAINTS
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct
            #endregion
        {
            #region VARIABLES
            readonly DBContext Context;
            #endregion

            #region CONSTRUCTORS
            public ModelList(DBContext context, DbSet<TModel> items) :
                base(items)
            {
                Context = context;
                Context.SaveChanges();
            }
            #endregion

            #region ADD MODELS
            protected override void AddModels(IEnumerable<TModel> items)
            {
                Items.AddRange(items);
            }
            #endregion

            #region ADD/ADD RANGE
            //-:cnd:noEmit
#if MODEL_APPENDABLE
            protected override void AddModel(TModel model)
            {
                Items.Add(model);
                Context.SaveChanges();
            }
#endif
            //+:cnd:noEmit
            #endregion

            #region DELETE/ DELETE RANGE
            //-:cnd:noEmit
#if MODEL_DELETABLE
            public override Task<bool> DeleteRange(IEnumerable<TModel>? items)
            {
                if(items == null) 
                    return Task.FromResult(false);
                try
                {
                    Items.RemoveRange(items);
                    Context.SaveChanges();
                    return Task.FromResult(true);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            protected override bool RemoveModel(TModel model)
            {
                Items.Remove(model);
                Context.SaveChanges();
                return (true);
            }
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
//-:cnd:noEmit
#endif
//+:cnd:noEmit
