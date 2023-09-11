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

using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common.Web.API
{
    #region CONTROLLER
    /// <summary>
    /// Generic controller. You must inherit this controller in your microservice project.
    /// We are using repository pattern so, we need a service repository
    /// to divert contract calls to it to perform contracted operations.
    /// </summary>
    /// <typeparam name="TModelDTO">Any model of your choice.</typeparam>
    [ApiController]
    [Route("[controller]")]
    public class Controller<TModelDTO, TModel, TID> : ControllerBase, IExController<TModelDTO, TModel, TID>
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
    {
        #region VARIABLES
        /// <summary>
        /// Since we are using repository pattern, we need service repository
        /// to divert contract calls to perform contract operations.
        /// Since this contro
        /// </summary>
        IService<TModelDTO, TModel, TID> service;
        #endregion

        #region CONSTRUCTORS
        public Controller(IService<TModelDTO, TModel, TID> _service)
        {
            service = _service;
        }
        #endregion

        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TID>.GetFirstModel() => service.GetFirstModel();
        #endregion

        #region GET MODEL COUNT
        int IModelCount.GetModelCount() => service.GetModelCount();
        #endregion

        #region GET MODEL/S
        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets a single model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to read.</param>
        /// <returns>Instance of TModelImplementation represented through TModelDTO</returns>
        [HttpGet("Get/{id}")]
        public async Task<TModelDTO> Get(TID id) =>
            await service.Get(id);
#endif
        //+:cnd:noEmit

        //-:cnd:noEmit
#if !MODEL_NONREADABLE
        /// <summary>
        /// Gets enumerable of model items.
        /// </summary>
        /// <param name="limitOfResult">If a number greater than zero is specified, then limits returned results up to that number, otherwise returns all.</param>
        /// <returns>IEnumerable of TModel</returns>
        [HttpGet("GetAll/{count}")]
        public async Task<IEnumerable<TModelDTO>> GetAll(int count = 0) =>
            await service.GetAll(count);

        /// <summary>
        /// Gets all models contained in this object picking from the index specified up to a count determined by limitOfResult.
        /// The count of models returned can be limited by the limitOfResult parameter.
        /// If the parameter value is zero, then all models are returned.
        /// </summary>
        /// <param name="startIndex">Start index which to start picking records from.</param>
        /// <param name="limitOfResult">Number to limit the number of models returned.</param>
        /// <returns>IEnumerable of models.</returns>
        [HttpGet("GetAll/{startIndex}, {count}")]
        public async Task<IEnumerable<TModelDTO>> GetAll(int startIndex, int count) =>
           await service.GetAll(startIndex, count);

        [HttpGet("FindAll")]
        public async Task<IEnumerable<TModelDTO>> FindAll([FromQuery][ModelBinder(BinderType =typeof(ParamBinder))] ISearchParameter parameter) =>
            await service.FindAll(parameter);
#endif
        //+:cnd:noEmit
        #endregion

        #region ADD ENTITY
        //-:cnd:noEmit
#if (MODEL_APPENDABLE)
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
        public async Task<TModelDTO> Add([ModelBinder(BinderType = typeof(ModelBinder))] TModelDTO model) =>
            await service.Add(model);
        async Task<TModelDTO> IAppendable<TModelDTO, TModel, TID>.Add(IModel model) =>
            await service.Add(model);
#endif
        //+:cnd:noEmit
        #endregion

        #region DELETE ENTITY
        //-:cnd:noEmit
#if (MODEL_DELETABLE)
        /// <summary>
        /// Deletes the model with the specified ID.
        /// </summary>
        /// <param name="id">ID of the model to delete.</param>
        /// <returns></returns>
        [HttpDelete("Delete/{id}")]
        public async Task<TModelDTO> Delete(TID id) =>
            await service.Delete(id);
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE ENTITY
        //-:cnd:noEmit
#if (MODEL_UPDATABLE)
        /// <summary>
        /// Updates a model specified by the given ID with the data of the given model.
        /// </summary>
        /// <param name="id">ID of the model to be updated.</param>
        /// <param name="model">
        /// Any model that implements the IModel interface and has all or a few data members identical to TModel.
        /// This allows DTOs to be used instead of an actual model object.
        /// </param>
        /// <param name="UpdateImmediately">If true, updates changes immediately, 
        /// otherwise you will have to call SaveChanges method manually.</param>
        /// <returns></returns>
        [HttpPut("Put/{id}")]
        public async Task<TModelDTO> Update(TID id, [ModelBinder(BinderType = typeof(ModelBinder))] TModelDTO model) =>
            await service.Update(id, model);

        async Task<TModelDTO> IUpdateable<TModelDTO, TModel, TID>.Update(TID id, IModel model) =>
            await service.Update(id, model);
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

