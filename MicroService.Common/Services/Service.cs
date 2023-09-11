/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Runtime.CompilerServices;

using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Services
{
    #region IService
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    public interface IService
    { }
    #endregion

    #region IService<TModelInterface, TModel, TIDType>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IService<TModelInterface, TModel, TIDType> : IService,
        IContract<TModelInterface, TModel, TIDType>
        #region TYPE CONSTRINTS
        where TModelInterface : IModel
        where TModel : Model<TIDType>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelInterface,
#endif
        //+:cnd:noEmit
        new()
        where TIDType : struct
        #endregion
    { }
    #endregion

    #region Service<TModelInterface, TModel, TIDType>
    /// <summary>
    /// This class represents a repository to be used in controller class.
    /// </summary>
    /// <typeparam name="TModelInterface">Model interface of your choice - must derived from IModel interface.</typeparam>
    /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
    public partial class Service<TModelInterface, TModel, TIDType, TModelCollection> : IService<TModelInterface, TModel, TIDType>
        #region TYPE CONSTRINTS
        where TModelInterface : IModel
        where TModel : Model<TIDType>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelInterface,
#endif
        //+:cnd:noEmit
        new()
        where TIDType : struct
        where TModelCollection : IModelCollection<TModel, TIDType>
        #endregion
    {
        #region VARIABLES
        protected readonly IModelCollection<TModel, TIDType> Context;
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TModelInterface);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public Service(TModelCollection _context)
        {
            Context = _context;
        }
        #endregion

        #region PROPERTIES
        #endregion

        #region GET ALL
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of Instance of TModelImplementations represented through TModelInterfaces.</returns>
        /// <exception cref="Exception"></exception>
        protected virtual Task<IEnumerable<TModel>> GetAll(int limitOfResult = 0)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));
            if (limitOfResult < 0)
                return Task.FromResult((IEnumerable<TModel>)new TModel[] { });

            if (limitOfResult > 0)
                return Task.FromResult(Context.Take(limitOfResult));
            return Task.FromResult((IEnumerable<TModel>)Context);
        }
        async Task<IEnumerable<TModelInterface>> IReadable<TModelInterface, TModel, TIDType>.GetAll(int limitOfResult) =>
            ToDTO(await GetAll(limitOfResult));

        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        protected Task<IEnumerable<TModel>> GetAll(int startIndex, int limitOfResult)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));

            if (limitOfResult < 0)
                return Task.FromResult((IEnumerable<TModel>)new TModel[] { });

            if (limitOfResult > 0)
                return Task.FromResult(Context.Skip(startIndex).Take(limitOfResult));
            else
                return Task.FromResult(Context.Skip(startIndex));
        }
        async Task<IEnumerable<TModelInterface>> IReadable<TModelInterface, TModel, TIDType>.GetAll(int startIndex, int limitOfResult) => 
            ToDTO(await GetAll(startIndex, limitOfResult));

        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TModelInterface</returns>
        /// <exception cref="Exception"></exception>
        protected virtual async Task<TModel> Get(TIDType id)
        {
            var result = await Context.Find(id);
            if (result == null)
                throw new Exception(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name));
            return result;
        }
        async Task<TModelInterface> IReadable<TModelInterface, TModel, TIDType>.Get(TIDType id) =>
           ToDTO(await Get(id));

        protected Task<IEnumerable<TModel>> FindAll(ISearchParameter parameter)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));
            return Context.FindAll(parameter);
        }
        async Task<IEnumerable<TModelInterface>> IReadable<TModelInterface, TModel, TIDType>.FindAll(ISearchParameter parameter) =>
            ToDTO(await FindAll(parameter));
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD
        //-:cnd:noEmit
#if (MODEL_APPENDABLE)
        /// <summary>
        /// Adds a specified model.
        /// </summary>
        /// <param name="model">Model to add.</param>
        /// <returns>Task with type of Model as result.</returns>
        /// <exception cref="Exception"></exception>
        protected async Task<TModel> Add(IModel model)
        {
            if (model == null) throw new Exception(string.Format("Null {0} value is not allowed!", typeof(TModelInterface).Name));
            TModel result;
            if (model is TModel)
                result = (TModel)model;
            else
            {
                result = new TModel();
                await ((IExCopyable)result).CopyFrom(model);
            }
            bool success = await Context.Add(result);
            if(success)
                return result;
            return default(TModel);
        }
        async Task<TModelInterface> IAppendable<TModelInterface, TModel, TIDType>.Add(IModel entity)
        {
            var result = await Add(entity);
#if MODEL_USEDTO
            if (NeedToUseDTO)
                return (TModelInterface)((IExModel)result).ToDTO(DTOType);
            return (TModelInterface)(object)result;
#else
            return result;
#endif
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region DELETE
        //-:cnd:noEmit
#if (MODEL_DELETABLE)
        /// <summary>
        /// Deltes a specified model.
        /// </summary>
        /// <param name="modelInterface">Model to delete.</param>
        /// <returns>Task with type of Model as result.</returns>
        /// <exception cref="Exception"></exception>
        protected async Task<TModel> Delete(TIDType id)
        {
            var model = await Context.Find(id);
            if (model == null)
                throw new Exception(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name));
            var result = await Context.Delete(model);
            if(result)
                return model;
            return default(TModel);
        }
        async Task<TModelInterface> IDeleteable<TModelInterface, TModel, TIDType>.Delete(TIDType id)
        {
            var result = await Delete(id);
#if MODEL_USEDTO
            if (NeedToUseDTO)
                return (TModelInterface)((IExModel)result).ToDTO(DTOType);
            return (TModelInterface)(object)result;
#else
            return result;
#endif
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE
        //-:cnd:noEmit
#if (MODEL_UPDATABLE)
        /// <summary>
        /// Updates a specified model.
        /// </summary>
        /// <param name="modelInterface">Model to update.</param>
        /// <returns>Task with type of Model as result.</returns>
        /// <exception cref="Exception"></exception>
        protected async Task<TModel> Update(TIDType id, IModel model)
        {
            var result = await Context.Find(id);
            if (result == null)
                throw new Exception(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name));

            if (await ((IExCopyable)result).CopyFrom(model))
            {
                await Context.SaveChanges();
                return result;
            }
            return default(TModel);
        }
        async Task<TModelInterface> IUpdateable<TModelInterface, TModel, TIDType>.Update(TIDType id, IModel entity)
        {
            var result = await Update(id, entity);
#if MODEL_USEDTO
            if (NeedToUseDTO)
                return (TModelInterface)((IExModel)result).ToDTO(DTOType);
            return (TModelInterface)(object)result;
#else
            return result;
#endif
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TIDType>.GetFirstModel() =>
            Context.GetFirstModel();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            Context.GetModelCount();
        #endregion

        #region TO DTO
        /// <summary>
        /// Converts model to an apporiate object of TModelInterface type.
        /// </summary>
        /// <param name="model">Model to convert.</param>
        /// <returns>Converted model ito an apporiate object of TModelInterface type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected TModelInterface? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TModelInterface);
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
                return (TModelInterface)((IExModel)model).ToDTO(DTOType);
#endif
            //+:cnd:noEmit
            return (TModelInterface)(object)model;
        }

        /// <summary>
        /// Converts models to an apporiate objects of TModelInterface type.
        /// </summary>
        /// <param name="model">Models to convert.</param>
        /// <returns>Converted models to an apporiate objects of TModelInterface type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IEnumerable<TModelInterface> ToDTO(IEnumerable<TModel> models)
        {
            if (models == null)
                return default(IEnumerable<TModelInterface>);
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
                return (IEnumerable<TModelInterface>)models.Select(m => ((IExModel)m).ToDTO(DTOType));
#endif
            //+:cnd:noEmit

            return (IEnumerable<TModelInterface>)models;
        }
        #endregion
    }
    #endregion
}
