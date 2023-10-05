/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit

using MicroService.Common.Collections;
using MicroService.Common.CQRS;
using MicroService.Common.Models;
using MicroService.Common.Sets;
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

        #region CREATE
        IModels<TID, TModel> IModelContext.Create<TID, TModel>()
        {
            var list = new EntityList<TID, TModel>(this, Set<TModel>());
            SaveChanges();
            return list;
        }
        IModelQuery<TModel> IModelContext.Create<TModel>()
        {
            var list = new QueryList<TModel>(this, Set<TModel>());
            SaveChanges();
            return list;
        }

        IModelQuery<TModel> IModelContext.Create<TModel, TItems>(TItems items)
        {
            var list = new QueryList<TModel>(this, Set<TModel>());
            SaveChanges();
            return list;
        }
        #endregion

        #region ON MODEL CREATION
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly);
        }
        #endregion

        #region QUERY LIST
        /// <summary>
        /// Represents an object which holds a collection of models indirectly.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        class QueryList<TModel> : QueryModels<TModel, DbSet<TModel>>
            #region TYPE CONSTRAINTS
            where TModel : class, ISelfModel<TModel>, new()
            #endregion
        {
            #region CONSTRUCTORS
            public QueryList(DBContext context, DbSet<TModel> items) :
                base(items)
            {
                context.SaveChanges();
            }
            #endregion

            #region ADD MODELS
            protected override void AddModels(IEnumerable<TModel> items)
            {
                Items.AddRange(items);
            }
            #endregion
        } 
        #endregion

        #region ENTITY LIST
        /// <summary>
        /// Represents an object which holds a collection of models indirectly.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class EntityList<TID, TModel> : Models<TID, TModel, DbSet<TModel>>
            #region TYPE CONSTRAINTS
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct
            #endregion
        {
            #region VARIABLES
            readonly DBContext Context;
            #endregion

            #region CONSTRUCTORS
            public EntityList(DBContext context, DbSet<TModel> items) :
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
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
