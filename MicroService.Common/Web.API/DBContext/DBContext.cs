/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
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
        {
            
        }
        #endregion

        #region CREATE
        IModelSet<TID1, TModel1> IModelContext.Create<TID1, TModel1>()
        {
            var list = new EntityList<TID1, TModel1>(Set<TModel1>());
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

        #region SAVE CHANGES
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        async Task<bool> IModifiable.SaveChanges()
        {
            return await SaveChangesAsync() > 0;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ENTITY LIST
        /// <summary>
        /// Represents an object which holds a collection of models indirectly.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        class EntityList<TID, TModel> : ModelSet<TID, TModel, DbSet<TModel>>
            #region TYPE CONSTRAINTS
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct
            #endregion
        {

            #region CONSTRUCTORS
            public EntityList(DbSet<TModel> items):
                base(items)
            { }
            #endregion

            #region ADD/ADD RANGE
            //-:cnd:noEmit
#if MODEL_APPENDABLE
            protected override void AddModel(TModel model) =>
                Items.Add(model);
            protected override void AddModels(IEnumerable<TModel> items) =>
                Items.AddRange(items);
#endif
            //+:cnd:noEmit
            #endregion

            #region DELETE/ DELETE RANGE
            //-:cnd:noEmit
#if MODEL_DELETABLE
            public override Task<bool> DeleteRange(IEnumerable<TModel> items)
            {
                try
                {
                    Items.RemoveRange(items);
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
