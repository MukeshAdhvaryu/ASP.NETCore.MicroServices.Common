/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !MODEL_NONQUERYABLE
//+:cnd:noEmit
using MicroService.Common.Collections;
using MicroService.Common.Exceptions;
using System.Runtime.CompilerServices;

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.CQRS;

namespace MicroService.Common.Services
{
    #region IQueryService<TOutDTO, TModel>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    public interface IQueryService<TOutDTO, TModel> : IService,
        IQueryContract<TOutDTO, TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : ISelfModel<TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        #endregion
    { }
    #endregion

    #region QueryService<TOutDTO, TModel, TID, TContext>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    /// <typeparam name="TContext">Instance of DBContext or ModelCollection creator.</typeparam>
    public partial class QueryService<TOutDTO, TModel, TContext> : IQueryService<TOutDTO, TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TContext : IModelContext
        #endregion
    {
        #region VARIABLES
        readonly IExModelQuery<TModel> Models;
        readonly static IExModel DummyModel = (IExModel) new TModel();
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public QueryService(TContext _context) :
            this(_context.Create<TModel>())
        { }
        public QueryService(IModelQuery<TModel>? models)
        {
            if (!(models is IExModelQuery<TModel>))
            {
                throw new NotSupportedException("Context supplied, is not compitible with this service!");
            }
            Models = (IExModelQuery<TModel>)models;
        }
        #endregion

        #region PROPERTIES
        protected IModelQuery<TModel> InnerList => Models;
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if !MODEL_NONQUERYABLE
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
            if (Models.GetModelCount() == 0)
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
#if !MODEL_NONQUERYABLE
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
            if (Models.GetModelCount() == 0)
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
#if !MODEL_NONQUERYABLE
        protected virtual Task<TModel?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSuppliedException);

            if (Models.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);
            return Models.Find(parameters, conditionJoin);
        }
        async Task<TOutDTO?> IFind<TOutDTO, TModel>.Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin) =>
            ToDTO(await Find(parameters, conditionJoin));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameter)
        //-:cnd:noEmit
#if !MODEL_NONQUERYABLE
        protected virtual Task<IEnumerable<TModel>?> FindAll(ISearchParameter? parameter)
        {
            if (parameter == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSuppliedException);

            if (Models.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);
            return Models.FindAll(parameter);
        }
        async Task<IEnumerable<TOutDTO>?> IFind<TOutDTO, TModel>.FindAll(ISearchParameter? parameter) =>
            ToDTO(await FindAll(parameter));
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if !MODEL_NONQUERYABLE
        protected virtual Task<IEnumerable<TModel>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSuppliedException);
            if (Models.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFoundException);

            return Models.FindAll(parameters, conditionJoin);
        }

        async Task<IEnumerable<TOutDTO>?> IFind<TOutDTO, TModel>.FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin) =>
            ToDTO(await FindAll(parameters, conditionJoin));
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
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
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
