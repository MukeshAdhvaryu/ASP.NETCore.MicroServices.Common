/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Runtime.CompilerServices;

using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Services
{
    #region IService
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    public interface IService
    { }
    #endregion

    #region IService<TModelDTO, TModel, TID>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TModelDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IService<TModelDTO, TModel, TID> : IService,
        IContract<TModelDTO, TModel, TID>
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
    { }
    #endregion
}
