/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Collections;
using System.Reflection;

using MicroService.Common.Attributes;
using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

using Microsoft.EntityFrameworkCore;

namespace MicroService.Common.Web.API
{
    #region GENERIC DBCONTEXT
    public partial class DBContext<TModel, TID> : DbContext, IExModelCollection<TModel, TID>
        where TModel : Model<TID>, new()
        where TID : struct
    {
        #region VARIABLES
        DbSet<TModel> models;
        #endregion

        #region CONSTRUCTOR
        public DBContext(DbContextOptions<DBContext<TModel, TID>> options)
            : base(options)
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
                    SaveChanges();
                }
            }
        }
        #endregion

        #region MODELS
        public DbSet<TModel> Models
        {
            get => models;
            set => models = value;
        }
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() => models.FirstOrDefault();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() => models.Count();
        #endregion

        #region FIND
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        async Task<TModel?> IModelCollection<TModel, TID>.Find(TID id)
        {
            return await models.FindAsync(id);
        }
        Task<TModel?> IModelCollection<TModel, TID>.Find(IEnumerable<ISearchParameter> keys)
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

        Task<IEnumerable<TModel>> IModelCollection<TModel, TID>.FindAll(ISearchParameter key)
        {
            Predicate<TModel> func = (m) =>
            {
                return ((IMatch)m).IsMatch(key);
            };
            return Task.FromResult(this.Where(m => func(m)));
        }
#else
        /// <summary>
        /// Finds a model based on given keys.
        /// </summary>
        /// <param name="keys">Keys to be used to find the model.</param>
        /// <returns>Task with result of type TModel.</returns>
        async Task<TModel?> IExModelCollection<TModel, TID>.Find(TID id)
        {
            return await models.FindAsync(id);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        async Task<bool> IModelCollection<TModel, TID>.Add(TModel model)
        {
            await models.AddAsync(model);
            return await SaveChangesAsync() > 0;
        }

        async Task<bool> IModelCollection<TModel, TID>.AddRange(IEnumerable<TModel> models)
        {
            await this.models.AddRangeAsync(models);
            return await SaveChangesAsync() > 0;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region DELTE
        //-:cnd:noEmit
#if MODEL_DELETABLE
        async Task<bool> IModelCollection<TModel, TID>.Delete(TModel model)
        {
            models.Remove(model);
            return await SaveChangesAsync() > 0;
        }
        async Task<bool> IModelCollection<TModel, TID>.DeleteRange(IEnumerable<TModel> models)
        {
            this.models.RemoveRange(models);
            return await SaveChangesAsync() > 0;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE
        //-:cnd:noEmit
#if MODEL_UPDATABLE
        async Task<bool> IModelCollection<TModel, TID>.Update(TModel model)
        {
            models.Update(model);
            return await SaveChangesAsync() > 0;
        }
        async Task<bool> IModelCollection<TModel, TID>.UpdateRange(IEnumerable<TModel> models)
        {
            this.models.UpdateRange(models);
            return await SaveChangesAsync() > 0;
        }
#endif
        //+:cnd:noEmit
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

        #region ENUMERATOR
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        IEnumerator<TModel> IEnumerable<TModel>.GetEnumerator()
        {
            foreach (var item in models)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable<TModel>)this).GetEnumerator();
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion

}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
