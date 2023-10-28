/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit

using System.Text.Json;
using System.Text;

using MicroService.Common.CQRS;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

//-:cnd:noEmit
#if (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
using MicroService.Common.Parameters;
#endif
//+:cnd:noEmit

using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common.API
{
    #region CONTROLLER
    /// <summary>
    /// Generic controller. You must inherit this controller in your microservice project.
    /// We are using repository pattern so, we need a service repository
    /// to divert contract calls to it to perform contracted operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Any model of your choice.</typeparam>
    [ApiController]
    [Route("[controller]")]
    public class Controller<TOutDTO, TModel, TID, TInDTO> :
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        QueryController<TOutDTO, TModel, TID>
#else
        ExController<TInDTO, TModel>
#endif
        //+:cnd:noEmit
        , IContract<TOutDTO, TModel, TID>, IExController
        #region TYPE CONSTRINTS
        where TOutDTO : IModel, new()
        where TModel : class, ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        where TInDTO : IModel, new()
        #endregion
    {
        #region VARIABLES
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        protected readonly ICommand<TOutDTO, TModel, TID> Command;
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        public Controller(IContract<TOutDTO, TModel, TID> service):
            base(service.Query)
        {
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
            Command = service.Command;
#endif
            //+:cnd:noEmit
        }
#else
        public Controller(IContract<TOutDTO, TModel, TID> service)
        {
            //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
            Command = service.Command;
#endif
            //+:cnd:noEmit
        }
#endif
#endregion

        #region PROPERTIES
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        ICommand<TOutDTO, TModel, TID> IContract<TOutDTO, TModel, TID>.Command =>
            Command;
#endif
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        IQuery<TOutDTO, TModel, TID> IContract<TOutDTO, TModel, TID>.Query => Query;
#endif
        //+:cnd:noEmit
        #endregion

        #region GET NEW MODEL
        static TModel GetNewModel() =>
            new TModel();
        #endregion

        #region GET MODEL COUNT
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
        [HttpPost("Add")]
        public async Task<TOutDTO?> Add([DTOBinder]TInDTO? model)
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
        [HttpPost("Add")]
        public async Task<IActionResult> Add([ModelBinder(typeof(ModelBinder))]TInDTO? model)
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
        [HttpPut("Update/{id}")]
        public async Task<TOutDTO?> Update(TID id, [DTOBinder] TInDTO? model)
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
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(TID id, TInDTO? model)
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

        #region ADD BULK
        //-:cnd:noEmit
#if (MODEL_APPENDABLE) && MODEL_APPENDBULK
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
        [HttpPost("AddBulk/{models}")]
        public async Task<Tuple<IEnumerable<TOutDTO?>?, string>> AddRange([DTOBinder]IEnumerable<TInDTO?> models)
        {
            try
            {
                return await Command.AddRange(models);
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
        [HttpPost("AddBulk/{models}")]
        public async Task<IActionResult> AddRange([DTOBinder]IEnumerable<TInDTO?> models)
        {
            try
            {
                return Ok(await Command.AddRange(models));
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

        #region UPDATE BULK
        //-:cnd:noEmit
#if (MODEL_UPDATABLE) && MODEL_UPDATEBULK
#if !MODEL_USEACTION
        /// <summary>
        /// Updates models based on an enumerable of models specified.
        /// </summary>
        /// <param name="ids">An enumerable of ID to be used to update models matching those IDs from the model collection.</param>
        /// <param name="models">An enumerable of models to update the model collection.</param>
        /// <returns>Collection of models which are successfully updated and a message for those which are not.</returns>
        [HttpPut("UpdateBulk/{ids}/{models}")]
        public async Task<Tuple<IEnumerable<TOutDTO?>?, string>> UpdateRange([FromQuery]IEnumerable<TID>? ids, [DTOBinder] IEnumerable<TInDTO?>? models)
        {
            try
            {
                return await Command.UpdateRange(ids, models);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Updates models based on an enumerable of models specified.
        /// </summary>
        /// <param name="IDs">An enumerable of ID to be used to update models matching those IDs from the model collection.</param>
        /// <param name="models">An enumerable of models to update the model collection.</param>
        /// <returns>Collection of models which are successfully updated and a message for those which are not.</returns>
        [HttpPut("UpdateBulk/{ids}/{models}")]
        public async Task<IActionResult> UpdateRange([FromQuery] IEnumerable<TID>? IDs,[DTOBinder]IEnumerable<TInDTO?> models)
        {
            try
            {
                return Ok(await Command.UpdateRange(IDs, models));
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

        #region DELETE BULK
        //-:cnd:noEmit
#if (MODEL_DELETABLE) && MODEL_DELETEBULK
#if !MODEL_USEACTION

        /// <summary>
        /// Deletes new models based on an enumerable of IDs specified.
        /// </summary>
        /// <param name="IDs">An enumerable of ID to be used to delete models matching those IDs from the model collection.</param>
        /// <returns>Collection of models which are successfully deleted and a message for those which are not.</returns>        
        [HttpPut("DeleteBulk/{ids}")]
        public async Task<Tuple<IEnumerable<TOutDTO?>?, string>> DeleteRange([FromQuery] IEnumerable<TID>? IDs)
        {
            try
            {
                return await Command.DeleteRange(IDs);
            }
            catch
            {
                throw;
            }
        }
#else
        /// <summary>
        /// Deletes new models based on an enumerable of IDs specified.
        /// </summary>
        /// <param name="IDs">An enumerable of ID to be used to delete models matching those IDs from the model collection.</param>
        /// <returns>Collection of models which are successfully deleted and a message for those which are not.</returns>        
        HttpPut("DeleteBulk/{ids}")]
        public async Task<IActionResult> DeleteRange([FromQuery] IEnumerable<TID>? IDs)
        {
            try
            {
                return await Command.DeleteRange(IDs);
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

