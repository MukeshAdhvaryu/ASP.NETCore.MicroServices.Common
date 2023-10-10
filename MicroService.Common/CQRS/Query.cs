//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit
using MicroService.Common.Contexts;
using MicroService.Common.Exceptions;
using System.Runtime.CompilerServices;

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

namespace MicroService.Common.CQRS
{
    #region Query<TOutDTO, TModel>
    /// <summary>
    /// This class represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public class Query<TOutDTO, TModel> : IExQuery<TOutDTO, TModel>
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TModel>, new()
        where TOutDTO : IModel
        #endregion
    {
        #region VARIABLES
        readonly IExQuery<TModel> query;
        readonly static IExModel DummyModel = (IExModel)new TModel();
        //-:cnd:noEmit
#if MODEL_USEDTO
        static readonly Type DTOType = typeof(TOutDTO);
        static readonly bool NeedToUseDTO = !DTOType.IsAssignableFrom(typeof(TModel));
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public Query(IModelContext _context) :
            this(_context.CreateQuery<TModel>())
        { }
        public Query(IQuery<TModel>? models)
        {
            if (!(models is IExQuery<TModel>))
            {
                throw DummyModel.GetModelException(ExceptionType.InvalidContext);
            }
            query = (IExQuery<TModel>)models;
        }
        #endregion

        #region PROPERTIES
        protected IQuery<TModel> InnerQuery => query;
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of Instance of TModelImplementations represented through TModelInterfaces.</returns>
        /// <exception cref="Exception"></exception>
        public virtual async Task<IEnumerable<TOutDTO>?> GetAll(int count = 0)
        {
            if (query.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);

            if (count < 0)
                throw DummyModel.GetModelException(ExceptionType.NegativeFetchCount, count.ToString());

            return ToDTO(await query.GetAll(count));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (start, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        public async virtual Task<IEnumerable<TOutDTO>?> GetAll(int startIndex, int count)
        {
            if (query.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);

            if (count < 0)
                throw DummyModel.GetModelException(ExceptionType.NegativeFetchCount, count.ToString());

            return ToDTO(await query.GetAll(startIndex, count));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameters, conditionJoin = 0)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        public async virtual Task<TOutDTO?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0)
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);

            if (query.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);
            return ToDTO(await query.Find(parameters, conditionJoin));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameter)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        public async virtual Task<TOutDTO?> Find(ISearchParameter? parameter)
        {
            if (parameter == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);

            if (query.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);
            return ToDTO(await query.Find(parameter));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameter)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        public async virtual Task<IEnumerable<TOutDTO>?> FindAll(ISearchParameter? parameter)
        {
            if (parameter == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);

            if (query.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);
            return ToDTO(await query.FindAll(parameter));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        public async virtual Task<IEnumerable<TOutDTO>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin)
        {
            if (parameters == null)
                throw DummyModel.GetModelException(ExceptionType.NoParameterSupplied);
            if (query.GetModelCount() == 0)
                throw DummyModel.GetModelException(ExceptionType.NoModelsFound);

            return ToDTO(await query.FindAll(parameters, conditionJoin));
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET FIRST MODEL
        IModel? IFirstModel.GetFirstModel() =>
            query.GetFirstModel();
        TModel? IFirstModel<TModel>.GetFirstModel() =>
            query.GetFirstModel();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() =>
            query.GetModelCount();
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
        protected virtual TOutDTO? ToDTO(TModel? model)
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
        protected virtual IEnumerable<TOutDTO>? ToDTO(IEnumerable<TModel>? models)
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

    #region Query<TOutDTO, TModel, TID>
    /// <summary>
    /// This class represents an object that allows reading a single model or multiple models.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public class Query<TOutDTO, TModel, TID> : Query<TOutDTO, TModel>, IQuery<TOutDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TID, TModel>, new()
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        , TOutDTO
#endif
        //+:cnd:noEmit
        where TID : struct
        #endregion
    {
        #region CONSTRUCTORS
        public Query(IModelContext _context) :
            base(_context)
        { }
        public Query(IQuery<TModel>? models):
            base(models)
        {
        }
        #endregion

        #region GET MODEL BY ID
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TOutDTO</returns>
        /// <exception cref="Exception"></exception>
        public async Task<TOutDTO?> Get(TID? id)
        {
            if (InnerQuery.GetModelCount() == 0)
                throw GetModelException(ExceptionType.NoModelsFound);

            var result = await InnerQuery.Find(new SearchParameter("ID", id));
            if (result == null)
                throw GetModelException(ExceptionType.NoModelFoundForID, id.ToString());
            return ToDTO(result);
        }
#else
        protected async Task<TOutDTO?> Get(TID? id)
        {
            if (InnerQuery.GetModelCount() == 0)
                throw GetModelException(ExceptionType.NoModelsFound);

            var result = await InnerQuery.Find(new SearchParameter("ID", id));
            if (result == null)
                throw GetModelException(ExceptionType.NoModelFoundForID, id.ToString());
            return ToDTO(result);
        }
#endif
        //+:cnd:noEmit
        async Task<TOutDTO?> IFindByID<TOutDTO, TModel, TID>.Get(TID? id) =>
           await Get(id);
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
