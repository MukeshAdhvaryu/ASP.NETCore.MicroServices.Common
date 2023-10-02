/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Services;

//-:cnd:noEmit
#if MODEL_USEACTION
using MicroService.Common.Web.API.Interfaces;
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
    public class Controller<TOutDTO, TModel, TID, TInDTO> : ControllerBase, IExController
        //-:cnd:noEmit
#if !MODEL_USEACTION
        , IContract<TOutDTO, TModel, TID>
#else
        , IActionContract<TModel, TID>
#endif
        //+:cnd:noEmit

        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : Model<TID>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        where TInDTO: IModel
        #endregion
    {
        #region VARIABLES
        /// <summary>
        /// Since we are using repository pattern, we need service repository
        /// to divert contract calls to perform contract operations.
        /// Since this contro
        /// </summary>
        IService<TOutDTO, TModel, TID> service;
        #endregion

        #region CONSTRUCTORS
        public Controller(IService<TOutDTO, TModel, TID> _service)
        {
            service = _service;
        }
        #endregion

        #region PROPERTIES
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() => 
            service.GetFirstModel();
        IModel? IFirstModel.GetFirstModel() => 
            service.GetFirstModel();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() => 
            service.GetModelCount();
        #endregion

        #region GET MODEL BY ID
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TOutDTO</returns>
        [HttpGet("Get/{id}")]
        public async Task<TOutDTO> Get(TID id)
        {
            try
            {
                return await service.Get(id);
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
        public async Task<IActionResult> Get(TID id)
        {
            try
            {
                return Ok(await service.Get(id));
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
#if !MODEL_NONREADABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Gets enumerable of model items.
        /// </summary>
        /// <param name="count">If a number greater than zero is specified, then limits returned results up to that number, otherwise returns all.</param>
        /// <returns>IEnumerable of TModel</returns>
        [HttpGet("GetAll/{count}")]
        public async Task<IEnumerable<TOutDTO>> GetAll(int count = 0)
        {
            try
            {
               return await service.GetAll(count);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Gets all models contained in this object.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("GetAll/{count}")]
        public async Task<IActionResult> GetAll(int count = 0)
        {
            try
            {
                return Ok(await service.GetAll(count));
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
#if !MODEL_NONREADABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        [HttpGet("GetAll/{startIndex}, {count}")]
        public async Task<IEnumerable<TOutDTO>> GetAll(int startIndex, int count)
        {
            try
            {
                return await service.GetAll(startIndex, count);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>An instance of IActionResult.</returns>
        [HttpGet("GetAll/{startIndex}, {count}")]
        public async Task<IActionResult> GetAll(int startIndex, int count)
        {
            try
            {
                return Ok(await service.GetAll(count));
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
#if !MODEL_NONREADABLE
#if !MODEL_USEACTION

        [HttpGet("FindAll/parameter")]
        public async Task<IEnumerable<TOutDTO>> FindAll([FromQuery][ModelBinder(BinderType =typeof(ParamBinder))] ISearchParameter parameter)
        {
            try
            {
                return await service.FindAll(parameter);
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
        public async Task<IActionResult> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] ISearchParameter parameter)
        {
            try
            {
                return Ok(await service.FindAll(parameter));
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
#if !MODEL_NONREADABLE
#if !MODEL_USEACTION
        /// <summary>
        /// Finds all models matched based on given parameters.
        /// </summary>
        /// <param name="parameters">Parameters to be used to find the model.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        /// <param name="conditionJoin">Option from AndOr enum to join search conditions.</param>
        /// <returns>Task with result of collection of type TModel.</returns>
        [HttpGet("FindAll/{conditionJoin}")]
        public async Task<IEnumerable<TOutDTO>> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter> parameters, AndOr conditionJoin = AndOr.OR)
        {
            try
            {
                return await service.FindAll(parameters, conditionJoin);
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
        public async Task<IActionResult> FindAll([FromQuery][ModelBinder(BinderType = typeof(ParamBinder))] IEnumerable<ISearchParameter> parameters, AndOr conditionJoin = AndOr.OR)
        {
            try
            {
                return Ok(await service.FindAll(parameters, conditionJoin));
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

        #region ADD ENTITY
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
        public async Task<TOutDTO> Add([FromQuery][ModelBinder(BinderType = typeof(ModelBinder))]TInDTO model)
        {
            try
            {
                return await service.Add(model);
            }
            catch
            {
                throw;
            }
        }
        async Task<TOutDTO> IAppendable<TOutDTO, TModel, TID>.Add(IModel model)
        {
            try
            {
               return await service.Add(model);
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
        public async Task<IActionResult> Add([FromQuery][ModelBinder(BinderType = typeof(ModelBinder))] TInDTO model)
        {
            try
            {
                return Ok(await service.Add(model));
            }
            catch
            {
                throw;
            }
        }
        async Task<IActionResult> IAppendable<TModel, TID>.Add(IModel model)
        {
            try
            {
                return Ok(await service.Add(model));
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

        #region DELETE ENTITY
        //-:cnd:noEmit
#if (MODEL_DELETABLE)
#if !MODEL_USEACTION
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public async Task<TOutDTO> Delete(TID id)
        {
            try
            {
                return await service.Delete(id);
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
                return Ok(await service.Delete(id));
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

        #region UPDATE ENTITY
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
        public async Task<TOutDTO> Update(TID id, [FromQuery][ModelBinder(BinderType = typeof(ModelBinder))] TInDTO model)
        {
            try
            {
                return await service.Update(id, model);
            }
            catch
            {
                throw;
            }
        }
        async Task<TOutDTO> IUpdatable<TOutDTO, TModel, TID>.Update(TID id, IModel model)
        {
            try
            {
                return await service.Update(id, model);
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
        public async Task<IActionResult> Update(TID id, [FromQuery][ModelBinder(BinderType = typeof(ModelBinder))] TInDTO model)
        {
            try
            {
                return Ok(await service.Update(id, model));
            }
            catch
            {
                throw;
            }
        }

        async Task<IActionResult> IUpdatable<TModel, TID>.Update(TID id, IModel model)
        {
            try
            {
                return Ok(await service.Update(id, model));
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

