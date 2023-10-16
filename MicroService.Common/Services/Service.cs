/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Contexts;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) || (!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using MicroService.Common.CQRS;
#endif
//+:cnd:noEmit

namespace MicroService.Common.Services
{
    #region Service<TOutDTO, TModel, TID, TContext>
    /// <summary>
    /// This interface represents repository object to be used in controller class.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    /// <typeparam name="TContext">Instance which implements IModelContext.</typeparam>
    public partial class Service<TOutDTO, TModel, TID, TContext> : IContract<TOutDTO, TModel, TID>
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
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        readonly IQuery<TOutDTO, TModel, TID>? Query;
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        readonly ICommand<TOutDTO, TModel, TID>? Command;
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public Service(TContext _context)
        {
            //-:cnd:noEmit
#if !(MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
            goto CREATEQUERY;
#else
            Command = _context.CreateCommand<TOutDTO, TModel, TID>();
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = ((IExCommand<TOutDTO, TModel, TID>)Command).GetQueryObject();
            return;
#endif
#endif
            CREATEQUERY:
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = _context.CreateQuery<TOutDTO, TModel, TID>();
#endif
            return;
            //+:cnd:noEmit
        }

        //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
        public Service(ICommand<TOutDTO, TModel, TID> command)
        {
            Command = command;
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = ((IExCommand<TOutDTO, TModel, TID>)command).GetQueryObject();
#endif        
        }
        //+:cnd:noEmit
#endif
        #endregion

        #region ADD
        //-:cnd:noEmit
#if MODEL_APPENDABLE
        public Task<TOutDTO?> Add(IModel? model)
        {
            return Command.Add(model);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region UPDATE
        //-:cnd:noEmit
#if MODEL_UPDATABLE
        public Task<TOutDTO?> Update(TID id, IModel? model)
        {
            return Command.Update(id, model);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region DELETE
        //-:cnd:noEmit
#if MODEL_DELETABLE
        public Task<TOutDTO?> Delete(TID id)
        {
            return Command.Delete(id);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL(count)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        public Task<IEnumerable<TOutDTO>?> GetAll(int count = 0)
        {
            return Query.GetAll(count);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET ALL(startIndex, count)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        public Task<IEnumerable<TOutDTO>?> GetAll(int startIndex, int count)
        {
            return Query.GetAll(startIndex, count);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL(parameter)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
        public Task<IEnumerable<TOutDTO>?> FindAll(ISearchParameter? parameter)
        {
            return Query.FindAll(parameter);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND ALL(parameters, conditionJoin)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
        public Task<TOutDTO?> Find(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.AND)
        {
            return Query.Find(parameters, conditionJoin);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND(parameters, conditionJoin)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
        public Task<IEnumerable<TOutDTO>?> FindAll(IEnumerable<ISearchParameter>? parameters, AndOr conditionJoin = AndOr.AND)
        {
            return Query.FindAll(parameters, conditionJoin);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND (parameter)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE) && MODEL_SEARCHABLE
        public Task<TOutDTO?> Find(ISearchParameter? parameter)
        {
            return Query.Find(parameter);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region FIND BY ID(id)
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        public Task<TOutDTO?> Get(TID? id)
        {
            return Query.Get(id);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region GET MODEL COUNT
        public int GetModelCount()
        {
            //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
            return Command.GetModelCount();
#elif (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            return Query.GetModelCount();
#else
            return 0;
#endif
            //+:cnd:noEmit
        }
        #endregion

        #region GET FIRST MODEL
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        public TModel? GetFirstModel()
        {
            return Query.GetFirstModel();
        }

        IModel? IFirstModel.GetFirstModel()
        {
            return ((IFirstModel)Query).GetFirstModel();
        }
#endif
        //+:cnd:noEmit
        #endregion
    }
#endregion
}
