/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

namespace MicroService.Common.Services
{
    #region IService
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    public interface IService
    { }
    #endregion

    #region IService<TModelDTO, TModel, TID>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TModelDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IService<TModelDTO, TModel, TID> : IService,
        IContract<TModelDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TModelDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    { }
    #endregion

    #region Service<TModelDTO, TModel, TID>
    /// <summary>
    /// This class represents a repository to be used in controller class.
    /// </summary>
    /// <typeparam name="TModelDTO">Model interface of your choice - must derived from IModel interface.</typeparam>
    /// <typeparam name="TModel">Model implementation of your choice - must derived from Model class.</typeparam>
    public partial class Service<TModelDTO, TModel, TID, TModelCollection> : IService<TModelDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TModelDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TModelDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        where TModelCollection : IModelCollection<TModel, TID>
        #endregion
    {
        #region VARIABLES
        readonly IExModelCollection<TModel, TID> Context;
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TModelDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public Service(TModelCollection _context)
        {
            if (!(_context is IExModelCollection<TModel, TID>))
                throw new NotSupportedException("Supplied model collection is not compitible with this service repository!");
            Context = (IExModelCollection < TModel, TID >)_context;
            Models = _context;
        }
        #endregion

        #region PROPERTIES
        protected TModelCollection Models { get; private set; }
        #endregion

        #region GET MODEL BY ID
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TModelDTO</returns>
        /// <exception cref="Exception"></exception>
        protected virtual async Task<TModel> Get(TID id)
        {
            var result = await Context.Find(id);
            if (result == null)
                throw new Exception(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name));
            return result;
        }
        async Task<TModelDTO> IReadable<TModelDTO, TModel, TID>.Get(TID id) =>
           ToDTO(await Get(id));

#else
        protected virtual async Task<TModel> Get(TID id)
        {
            var result = await Context.Find(id);
            if (result == null)
                throw new Exception(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name));
            return result;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of Instance of TModelImplementations represented through TModelInterfaces.</returns>
        /// <exception cref="Exception"></exception>
        protected virtual Task<IEnumerable<TModel>> GetAll(int count = 0)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));
            if (count < 0)
                return Task.FromResult((IEnumerable<TModel>)new TModel[] { });

            if (count > 0)
                return Task.FromResult(Context.Take(count));
            return Task.FromResult((IEnumerable<TModel>)Context);
        }
        async Task<IEnumerable<TModelDTO>> IReadable<TModelDTO, TModel, TID>.GetAll(int limitOfResult) =>
            ToDTO(await GetAll(limitOfResult));

#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (start, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        protected Task<IEnumerable<TModel>> GetAll(int startIndex, int count)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));

            if (count < 0)
                return Task.FromResult((IEnumerable<TModel>)new TModel[] { });

            if (count > 0)
                return Task.FromResult(Context.Skip(startIndex).Take(count));
            else
                return Task.FromResult(Context.Skip(startIndex));
        }
        async Task<IEnumerable<TModelDTO>> IReadable<TModelDTO, TModel, TID>.GetAll(int startIndex, int count) =>
            ToDTO(await GetAll(startIndex, count));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameter)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        protected Task<IEnumerable<TModel>> FindAll(ISearchParameter parameter)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));
            return Context.FindAll(parameter);
        }
        async Task<IEnumerable<TModelDTO>> IReadable<TModelDTO, TModel, TID>.FindAll(ISearchParameter parameter) =>
            ToDTO(await FindAll(parameter));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        protected virtual Task<IEnumerable<TModel>> FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin)
        {
            if (!Context.Any())
                throw new Exception(string.Format("No {0} are found", typeof(TModel).Name));
            return Context.FindAll(parameters, conditionJoin);
            throw new NotImplementedException();
        }

        async Task<IEnumerable<TModelDTO>> IReadable<TModelDTO, TModel, TID>.FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin) =>
            ToDTO(await FindAll(parameters, conditionJoin));
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
            if (model == null) throw new Exception(string.Format("Null {0} value is not allowed!", typeof(TModelDTO).Name));
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
        async Task<TModelDTO> IAppendable<TModelDTO, TModel, TID>.Add(IModel entity)
        {
            var result = await Add(entity);
#if MODEL_USEDTO
            if (NeedToUseDTO)
                return (TModelDTO)((IExModel)result).ToDTO(DTOType);
            return (TModelDTO)(object)result;
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
        protected async Task<TModel> Delete(TID id)
        {
            var model = await Context.Find(id);
            if (model == null)
                throw new Exception(string.Format("No such {0} found with ID: " + id, typeof(TModel).Name));
            var result = await Context.Delete(model);
            if(result)
                return model;
            return default(TModel);
        }
        async Task<TModelDTO> IDeleteable<TModelDTO, TModel, TID>.Delete(TID id)
        {
            var result = await Delete(id);
#if MODEL_USEDTO
            if (NeedToUseDTO)
                return (TModelDTO)((IExModel)result).ToDTO(DTOType);
            return (TModelDTO)(object)result;
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
        protected async Task<TModel> Update(TID id, IModel model)
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
        async Task<TModelDTO> IUpdateable<TModelDTO, TModel, TID>.Update(TID id, IModel entity)
        {
            var result = await Update(id, entity);
#if MODEL_USEDTO
            if (NeedToUseDTO)
                return (TModelDTO)((IExModel)result).ToDTO(DTOType);
            return (TModelDTO)(object)result;
#else
            return result;
#endif
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() =>
            Context.GetFirstModel();
        IModel? IFirstModel.GetFirstModel() =>
            Context.GetFirstModel();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            Context.GetModelCount();
        #endregion

        #region TO DTO
        /// <summary>
        /// Converts model to an apporiate object of TModelDTO type.
        /// </summary>
        /// <param name="model">Model to convert.</param>
        /// <returns>Converted model ito an apporiate object of TModelDTO type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected TModelDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TModelDTO);
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
                return (TModelDTO)((IExModel)model).ToDTO(DTOType);
#endif
            //+:cnd:noEmit
            return (TModelDTO)(object)model;
        }

        /// <summary>
        /// Converts models to an apporiate objects of TModelDTO type.
        /// </summary>
        /// <param name="model">Models to convert.</param>
        /// <returns>Converted models to an apporiate objects of TModelDTO type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IEnumerable<TModelDTO> ToDTO(IEnumerable<TModel> models)
        {
            if (models == null)
                return default(IEnumerable<TModelDTO>);
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
                return models.Select(m => ToDTO(m));
#endif
            //+:cnd:noEmit

            return (IEnumerable<TModelDTO>)models;
        }
        #endregion
    }
    #endregion
}
