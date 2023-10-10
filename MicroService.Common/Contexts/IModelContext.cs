/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) ||(!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using MicroService.Common.CQRS;
#endif
//+:cnd:noEmit
using MicroService.Common.Models;

namespace MicroService.Common.Contexts
{
    #region IModelContext
    public interface IModelContext : IDisposable
    {
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE

        /// <summary>
        /// Creates new instance of ModelSet<TModel, TID>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model<typeparamref name="TID"/></typeparam>
        /// <typeparam name="TID">Type of TID</typeparam>
        /// <returns>An instance of ModelSet<TModel, TID></returns>
        ICommand<TID, TModel> CreateCommand<TID, TModel>()
            where TModel : class, ISelfModel<TID, TModel>, new()
            where TID : struct;
#endif
        //+:cnd:noEmit

        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        /// <summary>
        /// Creates new instance of QuerySet<TModel>.
        /// </summary>
        /// <typeparam name="TModel">Type of Model></typeparam>
        /// <returns>An instance of QuerySet<TModel, TID></returns>
        IQuery<TModel> CreateQuery<TModel>()
            where TModel : class, ISelfModel<TModel>, new();
#endif
        //+:cnd:noEmit

    }
    #endregion
}
