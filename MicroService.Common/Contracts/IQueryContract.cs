/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit
using MicroService.Common.Models;

namespace MicroService.Common.Interfaces
{
    #region IQueryContract<TOutDTO, TModel, TID>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    public interface IQueryContract<TOutDTO, TModel> : IContract, IFirstModel<TModel> , IFind<TOutDTO, TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : ISelfModel<TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        #endregion
    { }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
