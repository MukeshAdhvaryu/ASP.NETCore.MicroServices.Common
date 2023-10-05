/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using System.Collections;

using MicroService.Common.Collections;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.Sets;

namespace MicroService.Common.CQRS
{
    #region IModelCommand<TID, TModel>
    /// <summary>
    /// Represents an object which holds a enumerables of keyless models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model></typeparam>
    public partial interface IModelCommand<TID, TModel> : IModelSet<TModel>, IFirstModel<TModel, TID> 
        //-:cnd:noEmit
#if MODEL_DELETABLE
        , IDelete<TModel>
#endif
#if MODEL_APPENDABLE
        , IAdd<TModel>
#endif
#if MODEL_UPDATABLE
        , IUpdate<TModel>
#endif
        //+:cnd:noEmit
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel>, new()
        where TID : struct
        #endregion
    { }
    #endregion

    #region IExModelCommand<TID, TModel>
    internal partial interface IExModelCommand<TID, TModel> : IModelCommand<TID, TModel>
        #region TYPE CONSTRAINTS
        where TModel : class, ISelfModel<TID, TModel>, new()
        where TID : struct
        #endregion
    {
        bool IsFound(TModel model, out TModel? result, bool addOperation = false);
    }
    #endregion
}
