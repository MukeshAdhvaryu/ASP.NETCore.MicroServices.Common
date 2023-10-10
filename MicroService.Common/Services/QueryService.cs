/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
//+:cnd:noEmit
using MicroService.Common.Exceptions;
using System.Runtime.CompilerServices;

using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;
using MicroService.Common.CQRS;
using MicroService.Common.Contexts;

namespace MicroService.Common.Services
{
    #region IQueryService<TOutDTO, TModel>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    public interface IQueryService<TOutDTO, TModel> : IService,
        IQueryContract<TOutDTO, TModel>
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

    #region QueryService<TOutDTO, TModel, TContext>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TContext">Instance which implements IModelContext.</typeparam>
    public partial class QueryService<TOutDTO, TModel, TContext> : IQueryService<TOutDTO, TModel>
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
        IQuery<TOutDTO, TModel> Query;
        #endregion

        #region CONSTRUCTORS
        public QueryService(TContext _context)
        {
            Query = new Query<TOutDTO, TModel>(_context);
        }

        public QueryService(IQuery<TModel> query)
        {
            Query = new Query<TOutDTO, TModel>(query);
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
        public Task<IEnumerable<TOutDTO>?> FindAll(ISearchParameter? parameter)
        {
            return Query.FindAll(parameter);
        }
        #endregion

        #region FIND ALL(parameters, conditionJoin)
        public Task<TOutDTO?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.AND)
        {
            return Query.Find(parameters, conditionJoin);
        }
        #endregion

        #region FIND(parameters, conditionJoin)
        public Task<IEnumerable<TOutDTO>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.AND)
        {
            return Query.FindAll(parameters, conditionJoin);
        }
        #endregion

        #region FIND (parameter)
        public Task<TOutDTO?> Find(ISearchParameter? parameter)
        {
            return Query.Find(parameter);
        }
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
    #endregion
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
