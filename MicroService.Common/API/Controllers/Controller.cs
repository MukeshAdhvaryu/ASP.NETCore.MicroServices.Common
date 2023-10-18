/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
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
    #region IExController 
    /// <summary>
    /// This interface represents a contract of operations.
    internal interface IExController
    { }
    #endregion

    #region CONTROLLER
    /// <summary>
    /// Generic controller. You must inherit this controller in your microservice project.
    /// We are using repository pattern so, we need a service repository
    /// to divert contract calls to it to perform contracted operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Any model of your choice.</typeparam>
    [ApiController]
    [Route("[controller]")]
    public class Controller<TOutDTO, TModel, TID, TInDTO> : ControllerBase, IContract<TOutDTO, TModel, TID>, IExController
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
        where TInDTO : IModel
        #endregion
    {
        #region VARIABLES
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        protected readonly IQuery<TOutDTO, TModel, TID> Query;
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        protected readonly ICommand<TOutDTO, TModel, TID> Command;
#endif
        #endregion

        #region CONSTRUCTORS
        public Controller(IContract<TOutDTO, TModel, TID> service)
        {
            var Service = service;
            //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = Service.Query;
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
            Command = Service.Command;
#endif
            //+:cnd:noEmit
        }
        #endregion

        #region PROPERTIES
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        IQuery<TOutDTO, TModel, TID> IContract<TOutDTO, TModel, TID>.Query =>
            Query;
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        ICommand<TOutDTO, TModel, TID> IContract<TOutDTO, TModel, TID>.Command =>
            Command;
#endif
        //+:cnd:noEmit
        #endregion

        #region GET NEW MODEL
        static TModel GetNewModel() =>
            new TModel();
        #endregion

        #region GET MODEL COUNT
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        [HttpGet("GetCount")]
        public int GetModelCount() =>
            ((IModelCount)this).GetModelCount();
#endif
        //+:cnd:noEmit

        int IModelCount.GetModelCount()
        {
            //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
            return Command.GetModelCount();
#elif (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            return Query.GetModelCount();
#else
            return 0;
#endif
            //+:cnd:noEmit
        }
        #endregion

        #region GET FIRST MODEL
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        [HttpGet("GetFirst")]
        public TModel? GetFirstModel() =>
           ((IFirstModel<TModel>)this).GetFirstModel();
#endif
        //+:cnd:noEmit
        IModel? IFirstModel.GetFirstModel() =>
           ((IFirstModel<TModel>)this).GetFirstModel();
        TModel? IFirstModel<TModel>.GetFirstModel()
        {
            //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
            return ((IExCommand<TOutDTO, TModel, TID>)Command).GetFirstModel();
#elif (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            return Query.GetFirstModel();
#else
            return default(TModel?);
#endif
            //+:cnd:noEmit
        }
        #endregion

        #region GET MODEL BY ID
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
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
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL (Optional: count)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
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
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
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

        #region FIND (parameters, conditionJoin)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION
        [HttpGet("Find/{conditionJoin}")]
        public async Task<TOutDTO?> Find([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0)
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
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameter">Parameter to be used to find the model.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("Find/{conditionJoin}")]
        public async Task<IActionResult> Find([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = 0)
        {
            try
            {
                return Ok(await Query.Find(parameters, conditionJoin));
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
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION

        [HttpGet("FindAll/parameter")]
        public async Task<IEnumerable<TOutDTO>?> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] ISearchParameter? parameter)
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
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        [HttpGet("FindAll/{conditionJoin}")]
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
        [HttpGet("FindAll/{conditionJoin}")]
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

        #region FIND (parameter)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
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

        #region ADD
        //-:cnd:noEmit
#if (MODEL_APPENDABLE)
#if !MODEL_USEACTION
        /// <summary>
        /// Adds a new model based on the given model.
        /// If the given model is not TModel, then a new appropriate model will be created by copying data from the given model.
        /// </summary>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>Model that is added.</returns>
        [HttpPost("Post")]
        public async Task<TOutDTO?> Add([FromQuery][ModelBinder(BinderType = typeof(ModelBinder))]TInDTO? model)
        {
            try
            {
                return await Command.Add(model);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Adds a new model based on the given model.
        /// If the given model is not TModel, then a new appropriate model will be created by copying data from the given model.
        /// </summary>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpPost("Post")]
        public async Task<IActionResult> Add([FromQuery][ModelBinder(BinderType = typeof(ModelBinder))] TInDTO? model)
        {
            try
            {
                return Ok(await Command.Add(model));
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

        #region DELETE
        //-:cnd:noEmit
#if (MODEL_DELETABLE)
#if !MODEL_USEACTION
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public async Task<TOutDTO?> Delete(TID id)
        {
            try
            {
                return await Command.Delete(id);
            }
            catch
            {
                throw;
            }
        }        
#else
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(TID id)
        {
            try
            {
                return Ok(await Command.Delete(id));
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

        #region UPDATE
        //-:cnd:noEmit
#if (MODEL_UPDATABLE)
#if !MODEL_USEACTION
        /// <summary>
        /// Updates a model specified by the given ID with the data of the given model.
        /// </summary>
        /// <param name="id">ID of the model to be updated.</param>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// otherwise you will have to call SaveChanges method manually.</param>
        /// <returns></returns>
        [HttpPut("Put/{id}")]
        public async Task<TOutDTO?> Update(TID id, [FromQuery][ModelBinder(BinderType = typeof(ModelBinder))] TInDTO? model)
        {
            try
            {
                return await Command.Update(id, model);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Updates a model specified by the given ID with the data of the given model.
        /// </summary>
        /// <param name="id">ID of the model to be updated.</param>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpPut("Put/{id}")]
        public async Task<IActionResult> Update(TID id, [FromQuery][ModelBinder(BinderType = typeof(ModelBinder))] TInDTO? model)
        {
            try
            {
                return Ok(await Command.Update(id, model));
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
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

