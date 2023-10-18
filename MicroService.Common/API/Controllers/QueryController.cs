/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD && (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
//+:cnd:noEmit

using MicroService.Common.CQRS;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

//-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
using MicroService.Common.Parameters;
#endif
//+:cnd:noEmit

using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common.Web.API
{
    #region QueryController<TOutDTO, TModel>
    /// <summary>
    /// Generic controller. You must inherit this controller in your microservice project.
    /// We are using repository pattern so, we need a service repository
    /// to divert contract calls to it to perform contracted operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Any model of your choice.</typeparam>
    [ApiController]
    [Route("[controller]")]
    public class QueryController<TOutDTO, TModel> : ControllerBase, IExController, IQueryContract<TOutDTO, TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        #endregion
    {
        #region VARIABLES
        protected readonly IQuery<TOutDTO, TModel> Query;
        #endregion

        #region CONSTRUCTORS
        public QueryController(IQueryContract<TOutDTO, TModel> _service)
        {
            var service = _service;
            Query = service.Query;   
        }
        #endregion

        #region PROPERTIES
        IQuery<TOutDTO, TModel> IQueryContract<TOutDTO, TModel>.Query => Query;
        #endregion

        #region GET NEW MODEL
        static TModel GetNewModel() => 
            new TModel();
        #endregion

        #region GET MODEL COUNT
        [HttpGet("GetCount")]
        public int GetModelCount()
        {
            return Query.GetModelCount();
        }
        #endregion

        #region GET FIRST MODEL
        [HttpGet("GetFirst")]
        public TModel? GetFirstModel()
        {
            return Query.GetFirstModel();
        }
        IModel? IFirstModel.GetFirstModel() =>
            GetFirstModel();
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Gets enumerable of model items.
        /// </summary>
        /// <param name="count">If a number greater than zero is specified, then limits returned results up to that number, otherwise returns all.</param>
        /// <returns>IEnumerable of TModel</returns>
        [HttpGet("GetAll/{count}")]
        public async Task<IEnumerable<TOutDTO>?> GetAll(int count = 0)
        {
            try
            {
               return await Query.GetAll(count);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the count parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("GetAll/{count}")]
        public async Task<IActionResult> GetAll(int count = 0)
        {
            try
            {
                return Ok(await Query.GetAll(count));
            }
            catch
            {
                throw;
            }
        }
#endif
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (start, count)
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by count.
        /// The count of models returned can be limited by the count parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        [HttpGet("GetAll/{startIndex}, {count}")]
        public async Task<IEnumerable<TOutDTO>?> GetAll(int startIndex, int count)
        {
            try
            {
                return await Query.GetAll(startIndex, count);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by count.
        /// The count of models returned can be limited by the count parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="count">Number to limit the number of models returned.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("GetAll/{startIndex}, {count}")]
        public async Task<IActionResult> GetAll(int startIndex, int count)
        {
            try
            {
                return Ok(await Query.GetAll(startIndex, count));
            }
            catch
            {
                throw;
            }
        }
#endif
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameter)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION

        [HttpGet("Find/parameter")]
        public async Task<TOutDTO?> Find([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] ISearchParameter? parameter)
        {
            try
            {
                return await Query.Find(parameter);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("Find/parameter")]
        public async Task<IActionResult> Find([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] ISearchParameter? parameter)
        {
            try
            {
                return Ok(await Query.Find(parameter));
            }
            catch
            {
                throw;
            }
        }
#endif
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameter)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION

        [HttpGet("FindAll/parameter")]
        public async Task<IEnumerable<TOutDTO>?> FindAll([FromQuery][ModelBinder(BinderType =typeof(ParamBinder))] ISearchParameter? parameter)
        {
            try
            {
                return await Query.FindAll(parameter);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("FindAll/parameter")]
        public async Task<IActionResult> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] ISearchParameter? parameter)
        {
            try
            {
                return Ok(await Query.FindAll(parameter));
            }
            catch
            {
                throw;
            }
        }
#endif
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        [HttpGet("FindAll/parameters{conditionJoin}")]
        public async Task<IEnumerable<TOutDTO>?> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.OR)
        {
            try
            {
                return await Query.FindAll(parameters, conditionJoin);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("FindAll/{parameters}, {conditionJoin}")]
        public async Task<IActionResult> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.OR)
        {
            try
            {
                return Ok(await Query.FindAll(parameters, conditionJoin));
            }
            catch
            {
                throw;
            }
        }
#endif
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL (parameters)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Find the first model matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        [HttpGet("Find/{conditionJoin}")]
        public async Task<TOutDTO?> Find([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.OR)
        {
            try
            {
                return await Query.Find(parameters, conditionJoin);
            }
            catch
            {
                throw;
            }
        }        
#else
        /// <summary>
        /// Find the first model matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("Find/{parameters},{conditionJoin}")]
        public async Task<IActionResult> Find([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter> parameters, AndOr conditionJoin = AndOr.OR)
        {
            try
            {
                return Ok(await Query.FindAll(parameters, conditionJoin));
            }
            catch
            {
                throw;
            }
        }

#endif
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion

    #region QueryController<TOutDTO, TModel, TID>
    public class QueryController<TOutDTO, TModel, TID> : QueryController<TOutDTO, TModel>, IQueryContract<TOutDTO, TModel, TID>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
        #region VARIABLES
        protected readonly new IQuery<TOutDTO, TModel, TID> Query;
        #endregion

        #region CONSTRUCTORS
        public QueryController(IQueryContract<TOutDTO, TModel, TID> _service) 
            : base(_service)
        {
            Query = _service.Query;
        }
        #endregion

        #region PROPERTIES
        IQuery<TOutDTO, TModel, TID> IQueryContract<TOutDTO, TModel, TID>.Query => Query;
        #endregion

        #region GET MODEL BY ID
        //-:cnd:noEmit
#if !MODEL_USEACTION
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TOutDTO</returns>
        [HttpGet("Get/{id}")]
        public async Task<TOutDTO?> Get(TID? id)
        {
            try
            {
                return await Query.Get(id);
            }
            catch 
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(TID? id)
        {
            try
            {
                return Ok(await Query.Get(id));
            }
            catch
            {
                throw;
            }
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
