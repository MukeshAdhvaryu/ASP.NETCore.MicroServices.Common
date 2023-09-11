using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Services;

namespace MicroService.Common.Web.API.Interfaces
{
    #region IExController<TModelDTO, TModel, TID>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TModelDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    internal interface IExController<TModelDTO, TModel, TID> : IContract<TModelDTO, TModel, TID>
        //+:cnd:noEmit
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
        IService<TModelDTO, TModel, TID> Service { get; }
    }
    #endregion

    #region IExDBContext<TModel, TID>
    /// <summary>
    /// This interface represents a DbContext for entity operations.
    /// </summary>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    internal interface IExDBContext<TModel, TID> : IModelCollection<TModel, TID>
        //+:cnd:noEmit
        #region TYPE CONSTRINTS
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
    }
    #endregion

}
