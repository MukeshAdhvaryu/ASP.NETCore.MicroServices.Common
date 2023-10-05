/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Sets;

namespace MicroService.Common.CQRS
{
    #region IModelQuery<TModel>
    /// <summary>
    /// Represents an object which holds a enumerables of keyless models directly or indirectly.
    /// </summary>
    /// <typeparam name="TModel">Type of keyless Model></typeparam>
    public partial interface IModelQuery<TModel> : IModelSet<TModel> , IFind<TModel, TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        #endregion
    { }
    #endregion

    #region IModelQuery<TModel>
    internal partial interface IExModelQuery<TModel> : IModelQuery<TModel>, IExModelSet<TModel>
        #region TYPE CONSTRAINTS
        where TModel : ISelfModel<TModel>
        #endregion
    { }
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
