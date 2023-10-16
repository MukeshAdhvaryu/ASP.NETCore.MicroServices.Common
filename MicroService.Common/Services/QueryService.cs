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
        public QueryService(TContext _context)
        {
            Query = _context.CreateQuery<TOutDTO, TModel>(true);
        }
        public QueryService(IQuery<TOutDTO, TModel> query)
        {
            Query = query;
        }
        #endregion

        #region GET ALL(count)
        public Task<IEnumerable<TOutDTO>?> GetAll(int count = 0)
        {
            return Query.GetAll(count);
        }
        #endregion

        #region GET ALL(startIndex, count)
        public Task<IEnumerable<TOutDTO>?> GetAll(int startIndex, int count)
        {
            return Query.GetAll(startIndex, count);
        }
        #endregion

        #region FIND ALL(parameter)
        //-:cnd:noEmit
#if MODEL_SEARCHABLE
        public Task<IEnumerable<TOutDTO>?> FindAll(ISearchParameter? parameter)
        {
            return Query.FindAll(parameter);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL(parameters, conditionJoin)
        //-:cnd:noEmit
#if MODEL_SEARCHABLE
public Task<TOutDTO?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.AND)
        {
            return Query.Find(parameters, conditionJoin);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND(parameters, conditionJoin)
        //-:cnd:noEmit
#if MODEL_SEARCHABLE
public Task<IEnumerable<TOutDTO>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.AND)
        {
            return Query.FindAll(parameters, conditionJoin);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameter)
        //-:cnd:noEmit
#if MODEL_SEARCHABLE
public Task<TOutDTO?> Find(ISearchParameter? parameter)
        {
            return Query.Find(parameter);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET MODEL COUNT
        public int GetModelCount()
        {
            return Query.GetModelCount();
        }
        #endregion

        #region GET FIRST MODEL
        public TModel? GetFirstModel()
        {
            return Query.GetFirstModel();
        }
        IModel? IFirstModel.GetFirstModel()
        {
            return ((IFirstModel)Query).GetFirstModel();
        }
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
        #region CONSTRUCTORS
        public QueryService(TContext _context) :
            this(_context.CreateQuery<TOutDTO, TModel, TID>(true))
        { }
        public QueryService(IQuery<TOutDTO, TModel, TID> query) :
            base(query) 
        { }
        #endregion

        #region FIND BY ID(id)
        public Task<TOutDTO?> Get(TID? id) =>
            ((IFindByID<TOutDTO, TModel, TID>)Query).Get(id);
        #endregion
    }
    //+:cnd:noEmit
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
