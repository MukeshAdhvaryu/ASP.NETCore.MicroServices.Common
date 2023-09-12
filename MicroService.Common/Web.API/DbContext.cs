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
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

using Microsoft.EntityFrameworkCore;

namespace MicroService.API.Data
{
    #region GENERIC DBCONTEXT
    public partial class DBContext<TModel, TIDType> : DbContext, IModelCollection<TModel, TIDType>
        where TModel : Model<TIDType>, new()
        where TIDType : struct
    {
        #region VARIABLES
        DbSet<TModel> models;
        #endregion

        #region CONSTRUCTOR
        public DBContext(DbContextOptions<DBContext<TModel, TIDType>> options)
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
        TModel? IFirstModel<TModel, TIDType>.GetFirstModel() => models.FirstOrDefault();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() => models.Count();
        #endregion

        #region FIND
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        Task<TModel?> IModelCollection<TModel, TIDType>.Find(TIDType id) =>
            Task.FromResult(models.FindAsync(id).Result);
        Task<TModel?> IModelCollection<TModel, TIDType>.Find(IEnumerable<IParameter> keys)
        {
            Predicate<TModel> predicate = (m) =>
            {
                IMatch match = m;

                foreach (var key in keys)
                {
                    if (key == null)
                        continue;
                    if (!match.IsMatch(key.Name, key.Value))
                        return false;
                }
                return true;
            };
            return Task.FromResult(models.FirstOrDefault((m) => predicate(m)));
        }

        Task<IEnumerable<TModel>> IModelCollection<TModel, TIDType>.FindAll(IParameter key)
        {
            Func<TModel, bool> func = (m) =>
            {
                return ((IMatch)m).IsMatch(key.Name, key.Value);
            };
            return Task.FromResult((IEnumerable<TModel>)models.Where(m => func(m)));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        async Task<bool> IModelCollection<TModel, TIDType>.Add(TModel model)
        {
            await models.AddAsync(model);
            return await SaveChangesAsync() > 0;
        }

        async Task<bool> IModelCollection<TModel, TIDType>.AddRange(IEnumerable<TModel> models)
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
        async Task<bool> IModelCollection<TModel, TIDType>.Delete(TModel model)
        {
            models.Remove(model);
            return await SaveChangesAsync() > 0;
        }
        async Task<bool> IModelCollection<TModel, TIDType>.DeleteRange(IEnumerable<TModel> models)
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
        async Task<bool> IModelCollection<TModel, TIDType>.Update(TModel model)
        {
            models.Update(model);
            return await SaveChangesAsync() > 0;
        }
        async Task<bool> IModelCollection<TModel, TIDType>.UpdateRange(IEnumerable<TModel> models)
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
