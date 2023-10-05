/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

using MicroService.Common.Collections;
using MicroService.Common.Exceptions;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Sets;

namespace MicroService.Common.Services
{
    #region IService
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    public interface IService: IContract
    { }
    #endregion

    #region IService<TOutDTO, TModel, TID>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IService<TOutDTO, TModel, TID> : IService,
        IContract<TOutDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    { }
    #endregion

    #region Service<TOutDTO, TModel, TID, TContext>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    /// <typeparam name="TContext">Instance of DBContext or ModelCollection creator.</typeparam>
    public partial class Service<TOutDTO, TModel, TID, TContext> : IService<TOutDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        where TContext : IModelContext
        #endregion
    {
        #region VARIABLES
        readonly TContext Context;
        readonly IExModelSet<TID, TModel> Models;
        readonly static IExModel DummyModel = (IExModel) new TModel();
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public Service(TContext _context)
        {
            Context = _context;
            var models = Context.Create<TID, TModel>();
            if(!(models is IExModelSet<TID, TModel>))
            {
                throw new NotSupportedException("Context supplied, is not compitible with this service!");
            }
            Models = (IExModelSet<TID, TModel>)models;
        }
        #endregion

        #region PROPERTIES
        protected IModelSet<TID, TModel> InnerList => Models;
        #endregion

        #region GET MODEL BY ID
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TOutDTO</returns>
        /// <exception cref="Exception"></exception>
        protected virtual async Task<TModel?> Get(TID? id)
        {
            var result = await Models.Get(id);
            if (result == null)
                throw DummyModel.GetModelException(ExceptionType.NoModelFoundForIDException, id.ToString());
            return result;
        }
        async Task<TOutDTO?> IReadable<TOutDTO, TModel, TID>.Get(TID? id) =>
           ToDTO(await Get(id));

#else
        protected virtual async Task<TModel?> Get(TID id)
        {
            var result = await Models.Get(id);
            if (result == null)
                throw DummyModel.GetModelException(ExceptionType.NoModelFoundForIDException, id.ToString());
            return result;
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of Instance of TModelImplementations represented through TModelInterfaces.</returns>
        /// <exception cref="Exception"></exception>
        protected virtual Task<IEnumerable<TModel>?> GetAll(int count = 0)
        {
            if (!Models.Any())
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);

            if (count < 0)
                throw DummyModel.GetModelException(ExceptionType.NegativeFetchCountException, count.ToString());

            return Models.GetAll(count);
        }
        async Task<IEnumerable<TOutDTO>?> IFind<TOutDTO, TModel>.GetAll(int count) =>
            ToDTO(await GetAll(count));
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (start, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        protected Task<IEnumerable<TModel>?> GetAll(int startIndex, int count)
        {
            if (!Models.Any())
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);

            if (count < 0)
                throw DummyModel.GetModelException(ExceptionType.NegativeFetchCountException, count.ToString());

            return Models.GetAll(startIndex, count);
        }
        async Task<IEnumerable<TOutDTO>?> IFind<TOutDTO, TModel>.GetAll(int startIndex, int count) =>
            ToDTO(await GetAll(startIndex, count));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        protected virtual Task<TModel?> Find(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSuppliedException);

            if (!Models.Any())
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);
            return Models.Find(parameters, conditionJoin);
        }
        async Task<TOutDTO?> IFind<TOutDTO, TModel>.Find(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin) =>
            ToDTO(await Find(parameters, conditionJoin));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameter)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        protected virtual Task<IEnumerable<TModel>?> FindAll(ISearchParameter parameter)
        {
            if (parameter == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSuppliedException);

            if (!Models.Any())
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);
            return Models.FindAll(parameter);
        }
        async Task<IEnumerable<TOutDTO>?> IFind<TOutDTO, TModel>.FindAll(ISearchParameter parameter) =>
            ToDTO(await FindAll(parameter));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE && !MODEL_NONQUERYABLE
        protected virtual Task<IEnumerable<TModel>?> FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSuppliedException);
            if (!Models.Any())
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);

            return Models.FindAll(parameters, conditionJoin);
        }

        async Task<IEnumerable<TOutDTO>?> IFind<TOutDTO, TModel>.FindAll(IEnumerable<ISearchParameter> parameters, AndOr conditionJoin) =>
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
        protected virtual async Task<TModel?> Add(IModel model)
        {
            if (model == null)
                throw DummyModel.GetModelException(ExceptionType.NoModelSuppliedException);
            TModel result;
            if (model is TModel)
                result = (TModel)model;
            else
            {
                result = new TModel();
                bool ok = await ((IExCopyable)result).CopyFrom(model);
                if (!ok)
                    throw DummyModel.GetModelException(ExceptionType.ModelCopyOperationFailed, model.ToString());
            }
            try
            {
                await Models.Add(result);
                await Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw DummyModel.GetModelException(ExceptionType.AddOperationFailedException, null, e);

            }
            return result;
        }
        async Task<TOutDTO?> IAppendable<TOutDTO, TModel, TID>.Add(IModel model) =>
            ToDTO(await Add(model));
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
        protected async Task<TModel?> Delete(TID id)
        {
            var model = await Models.Get(id);
            if (model == null)
                throw DummyModel.GetModelException(ExceptionType.NoModelFoundForIDException, id.ToString());
            try
            {
                var result = await Models.Delete(model);
                if (result)
                {
                    await Context.SaveChanges();
                    return model;
                }
            }
            catch (Exception e)
            {
                throw DummyModel.GetModelException(ExceptionType.DeleteOperationFailedException, id.ToString(), e);
            }
            throw DummyModel.GetModelException(ExceptionType.DeleteOperationFailedException, id.ToString());
        }
        async Task<TOutDTO?> IDeleteable<TOutDTO, TModel, TID>.Delete(TID id) =>
            ToDTO(await Delete(id));
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
        protected async Task<TModel?> Update(TID id, IModel model)
        {
            var result = await Models.Get(id);
            if (result == null)
                throw DummyModel.GetModelException(ExceptionType.NoModelFoundForIDException, id.ToString());

            bool ok = await ((IExCopyable)result).CopyFrom(model);
            if (!ok)
                throw DummyModel.GetModelException(ExceptionType.ModelCopyOperationFailed, model.ToString());
            try
            {
                await Context.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                throw DummyModel.GetModelException(ExceptionType.UpdateOperationFailedException, id.ToString(), e);
            }
            throw DummyModel.GetModelException(ExceptionType.UpdateOperationFailedException, id.ToString());
        }
        async Task<TOutDTO?> IUpdatable<TOutDTO, TModel, TID>.Update(TID id, IModel entity) =>
           ToDTO(await Update(id, entity));
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() =>
            Models.GetFirstModel();
        IModel? IFirstModel.GetFirstModel() =>
            Models.GetFirstModel();
        TModel? IFirstModel<TModel>.GetFirstModel() =>
            Models.GetFirstModel();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            Models.GetModelCount();
        #endregion

        #region GET MODEL EXCEPTION
        /// <summary>
        /// Supplies an appropriate exception for a failure in a specified method.
        /// </summary>
        /// <param name="exceptionType">Type of exception to get.</param>
        /// <param name="additionalInfo">Additional information to aid the task of exception supply.</param>
        /// <param name="innerException">Inner exception which is already thrown.</param>
        /// <returns>Instance of SpecialException class.</returns>
        protected ModelException GetModelException(ExceptionType exceptionType, string? additionalInfo = null, Exception? innerException = null) =>
            DummyModel.GetModelException(exceptionType, additionalInfo, innerException);
        #endregion

        #region TO DTO
        /// <summary>
        /// Converts model to an apporiate object of TOutDTO type.
        /// </summary>
        /// <param name="model">Model to convert.</param>
        /// <returns>Converted model ito an apporiate object of TOutDTO type.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected TOutDTO? ToDTO(TModel? model)
        {
            if (model == null)
                return default(TOutDTO);
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
            {
                var result = ((IExModel)model).ToDTO(DTOType);
                if (result == null)
                    return default(TOutDTO);
                return (TOutDTO?)((IExModel)model).ToDTO(DTOType);
            }
#endif
            //+:cnd:noEmit
            return (TOutDTO?)(object)model;
        }

        /// <summary>
        /// Converts models to an apporiate objects of TOutDTO type.
        /// </summary>
        /// <param name="model">Models to convert.</param>
        /// <returns>Converted models to an apporiate objects of TOutDTO type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IEnumerable<TOutDTO>? ToDTO(IEnumerable<TModel>? models)
        {
            if (models == null)
                return Enumerable.Empty<TOutDTO>();
            //-:cnd:noEmit
#if (MODEL_USEDTO)
            if (NeedToUseDTO)
                return (IEnumerable<TOutDTO>?)models.Select(m => ToDTO(m));
#endif
            //+:cnd:noEmit

            return (IEnumerable<TOutDTO>?)models;
        }
        #endregion

        #region DISPOSE
        void IDisposable.Dispose()
        {
            Context?.Dispose();
        }
        #endregion
    }
    #endregion
}
