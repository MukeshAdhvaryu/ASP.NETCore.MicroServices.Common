/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Contexts;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;
using MicroService.Common.Parameters;

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
        readonly IQuery<TOutDTO, TModel, TID> Query;
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        readonly IExCommand<TOutDTO, TModel, TID> Command;
#endif
        //+:cnd:noEmit
        #endregion

        #region CONSTRUCTORS
        public Service(TContext _context, ICollection<TModel>? source = null)
        {
            //-:cnd:noEmit
#if !(MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
            goto CREATEQUERY;
#else
            Command = (IExCommand<TOutDTO, TModel, TID>)_context.CreateCommand<TOutDTO, TModel, TID>(source: source);
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = Command.GetQueryObject();
            return;
#endif
#endif
            CREATEQUERY:
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = _context.CreateQuery<TOutDTO, TModel, TID>(source: source);
#endif
            return;
            //+:cnd:noEmit
        }

        //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
        public Service(ICommand<TOutDTO, TModel, TID> command)
        {
            Command = (IExCommand<TOutDTO, TModel, TID>)command;
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            Query = Command.GetQueryObject();
#endif        
        }
        //+:cnd:noEmit
#endif
        #endregion

        #region PROPERTIES
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        IQuery<TOutDTO, TModel, TID> IContract<TOutDTO, TModel, TID>.Query => Query;
#endif
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        ICommand<TOutDTO, TModel, TID> IContract<TOutDTO, TModel, TID>.Command => Command;
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
        public TModel? GetFirstModel()
        {
            //-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
            return Command.GetFirstModel();
#elif (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
            return Query.GetFirstModel();
#else
            return default(TModel?);
#endif
            //+:cnd:noEmit
        }
        IModel? IFirstModel.GetFirstModel() =>
            GetFirstModel();
        #endregion
    }
    #endregion
}
