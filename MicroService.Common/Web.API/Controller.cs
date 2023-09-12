/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using System.Reflection;

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

using MicroService.Common.Services;

using Microsoft.AspNetCore.Mvc;

namespace MicroService.API.Controllers
{
    #region CONTROLLER
    /// <summary>
    /// Generic controller. You must inherit this controller in your microservice project.
    /// We are using repository pattern so, we need a service repository
    /// to divert contract calls to it to perform contracted operations.
    /// </summary>
    /// <typeparam name="TModelInterface">Any model of your choice.</typeparam>
    [ApiController]
    [Route("[controller]")]
    public class Controller<TModelInterface, TModel, TIDType> : ControllerBase, IContract<TModelInterface, TModel, TIDType>
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
    {
        #region VARIABLES
        /// <summary>
        /// Since we are using repository pattern, we need service repository
        /// to divert contract calls to perform contract operations.
        /// Since this contro
        /// </summary>
        IService<TModelInterface, TModel, TIDType> service;
        #endregion

        #region CONSTRUCTORS
        public Controller(IService<TModelInterface, TModel, TIDType> _service)
        {
            service = _service;
        }
        #endregion


        #region GET FIRST MODEL
        TModel? IFirstModel<TModel, TIDType>.GetFirstModel() => service.GetFirstModel();
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
        /// <returns>Instance of TModelImplementation represented through TModelInterface</returns>
        [HttpGet("Get/{id}")]
        public async Task<TModelInterface> Get(TIDType id) =>
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
        public async Task<IEnumerable<TModelInterface>> GetAll(int count = 0) =>
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
        public async Task<IEnumerable<TModelInterface>> GetAll(int startIndex, int count) =>
           await service.GetAll(startIndex, count);
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
        public async Task<TModelInterface> Add([ModelBinder(BinderType = typeof(ModelBinder))] TModelInterface model) =>
            await service.Add(model);
        async Task<TModelInterface> IAppendable<TModelInterface, TModel, TIDType>.Add(IModel model) =>
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
        public async Task<TModelInterface> Delete(TIDType id) =>
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
        public async Task<TModelInterface> Update(TIDType id, [ModelBinder(BinderType = typeof(ModelBinder))] TModelInterface model) =>
            await service.Update(id, model);

        async Task<TModelInterface> IUpdateable<TModelInterface, TModel, TIDType>.Update(TIDType id, IModel model) =>
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

