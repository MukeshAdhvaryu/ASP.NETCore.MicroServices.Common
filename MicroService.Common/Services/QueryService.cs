/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit

using MicroService.Common.Contexts;
using MicroService.Common.CQRS;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

namespace MicroService.Common.Services
{
    #region QueryService<TOutDTO, TModel, TContext>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TContext">Instance which implements IModelContext.</typeparam>
    public partial class QueryService<TOutDTO, TModel, TContext> : IQueryContract<TOutDTO, TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TContext : IModelContext
        #endregion
    {
        #region VARIABLES
        protected readonly IQuery<TOutDTO, TModel> Query;
        #endregion

        #region CONSTRUCTORS
        public QueryService(TContext _context, ICollection<TModel>? source = null)
        {
            Query = _context.CreateQuery<TOutDTO, TModel>(true, source);
        }
        public QueryService(IQuery<TOutDTO, TModel> query)
        {
            Query = query;
        }
        #endregion

        #region PROPERTIES
        IQuery<TOutDTO, TModel> IQueryContract<TOutDTO, TModel>.Query => Query;
        #endregion

        #region GET MODEL COUNT
        public int GetModelCount()
        {
            return Query.GetModelCount();
        }
        #endregion

        #region GET FIRST MODEL
        public TModel? GetFirstModel() =>
            Query.GetFirstModel();
        IModel? IFirstModel.GetFirstModel() =>
            Query.GetFirstModel();
        #endregion

    }
    //+:cnd:noEmit
    #endregion

    #region QueryService<TOutDTO, TModel, TContext>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TContext">Instance which implements IModelContext.</typeparam>
    public partial class QueryService<TOutDTO, TModel, TID, TContext> :
        QueryService<TOutDTO, TModel, TContext>, IQueryContract<TOutDTO, TModel, TID> 
        #region TYPE CONSTRINTS
        where TOutDTO : IModel
        where TModel : class, ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        where TContext : IModelContext
        #endregion
    {
        #region VARIABLES
        protected readonly new IQuery<TOutDTO, TModel, TID> Query;
        #endregion

        #region CONSTRUCTORS
        public QueryService(TContext _context, ICollection<TModel>? source = null) :
            this(_context.CreateQuery<TOutDTO, TModel, TID>(true, source))
        {
            Query =(IQuery<TOutDTO, TModel, TID>) ((IQueryContract<TOutDTO, TModel>)this).Query;
        }
        public QueryService(IQuery<TOutDTO, TModel, TID> query) :
            base(query) 
        {
            Query = (IQuery<TOutDTO, TModel, TID>)((IQueryContract<TOutDTO, TModel>)this).Query;
        }
        #endregion

        #region PROPERTIES
        IQuery<TOutDTO, TModel, TID> IQueryContract<TOutDTO, TModel, TID>.Query => Query;
        #endregion
    }
    //+:cnd:noEmit
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
