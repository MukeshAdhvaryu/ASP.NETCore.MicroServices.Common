/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
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

    #region IService<TModelInterface, TModel, TIDType>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TModelInterface">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TIDType">Primary key type of the model.</typeparam>
    public interface IService<TModelInterface, TModel, TIDType> : IService,
        IContract<TModelInterface, TModel, TIDType>
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
    { }
    #endregion
}
