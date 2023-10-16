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
using MicroService.Common.Interfaces;

namespace MicroService.Common.Contexts
{
    #region ModelContext
    /// <summary>
    /// Represents an object which creates list based model context useful for TDD.
    /// </summary>
    public sealed  partial class ModelContext : IModelContext
    {
        #region CREATE COMMAND
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        ICommand<TOutDTO, TModel, TID> IModelContext.CreateCommand<TOutDTO, TModel, TID>(bool initialzeData)
        {
            return new CommandObject<TOutDTO, TModel, TID>(null, initialzeData);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region CREATE QUERY
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        IQuery<TOutDTO, TModel> IModelContext.CreateQuery<TOutDTO, TModel>(bool initialzeData)
        {
            return new QueryObject<TOutDTO, TModel>(null, initialzeData);
        }
        IQuery<TOutDTO, TModel, TID> IModelContext.CreateQuery<TOutDTO, TModel, TID>(bool initialzeData)
        {
            return new QueryObject<TOutDTO, TModel, TID>(null, initialzeData);
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region DISPOSE
        public void Dispose() { }
        #endregion
    }
    #endregion

    #region ModelContext
    partial class ModelContext
    {
        #region COMMAND CLASS IMPLEMENTATION
        //-:cnd:noEmit
#if MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE
        class CommandObject<TOutDTO, TModel, TID> : Command<TOutDTO, TModel, TID>
            #region TYPE CONSTRINTS
            where TOutDTO : IModel
            where TModel : class, ISelfModel<TID, TModel>, new()
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            , TOutDTO
#endif
            //+:cnd:noEmit
            where TID : struct
            #endregion
        {
            List<TModel> models;

            #region CONSTRUCTORS
            public CommandObject(List<TModel>? _models = null, bool initializeData = true)
            {
                models = _models?? new List<TModel>();
                if (!initializeData)
                    return;

                IEnumerable<TModel>? items = GetInitialData();
                if (items == null)
                    return;
                models.AddRange(items);
            }
            #endregion

            #region ADD
            //-:cnd:noEmit
#if (MODEL_APPENDABLE)
            protected override void AddModel(TModel model)
            {
                models.Add(model);
            }
#endif
            //+:cnd:noEmit
            #endregion

            #region DELETE
            //-:cnd:noEmit
#if (MODEL_DELETABLE)
            protected override bool DeleteModel(TModel model)
            {
                return models.Remove(model);
            }
#endif
            //+:cnd:noEmit
            #endregion

            #region GET QUERY OBJECT
            //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
            protected override IQuery<TOutDTO, TModel, TID> GetQueryObject()
            {
                return new QueryObject<TOutDTO, TModel, TID>(models, false);
            }
#endif
            //+:cnd:noEmit
            #endregion

            #region GET MODEL COUNT
            public override int GetModelCount() => models.Count;
            #endregion

            #region GET BY ID
            protected override Task<TModel?> Get(TID id) =>
              Task.FromResult(models.FirstOrDefault(m => Equals(m.ID, id)));
            #endregion
        }
#endif
        //+:cnd:noEmit
        #endregion

        #region QUERY CLASS IMPLEMENTATION
        //-:cnd:noEmit
#if !MODEL_NONREADABLE || !MODEL_NONQUERYABLE
        class QueryObject<TOutDTO, TModel, TID> : Query<TOutDTO, TModel>, IQuery<TOutDTO, TModel, TID>
            #region TYPE CONSTRINTS
            where TOutDTO : IModel
            where TModel : class, ISelfModel<TID, TModel>, new()
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
            , TOutDTO
#endif
            //+:cnd:noEmit
            where TID : struct
            #endregion
        {
            List<TModel> models;

            #region CONSTRUCTORS
            public QueryObject(List<TModel>? _models = null, bool initialzeData = false)
            {
                models = _models ?? new List<TModel>();
                if (!initialzeData)
                    return;
                IEnumerable<TModel>? items = GetInitialData();
                if (items == null)
                    return;
                models.AddRange(items);
            }
            #endregion

            #region ITEMS
            protected override IEnumerable<TModel> GetItems() => models;
            #endregion

            #region GET MODEL COUNT
            public override int GetModelCount() => models.Count;
            #endregion

            #region GET MODEL BY ID
            Task<TOutDTO?> IFindByID<TOutDTO, TModel, TID>.Get(TID? id)
            {
                if (GetModelCount() == 0)
                    throw GetModelException(ExceptionType.NoModelsFound);

                TModel? result = models.FirstOrDefault(m => Equals(m.ID, id));

                if (result == null)
                    throw GetModelException(ExceptionType.NoModelFoundForID, id.ToString());
                return Task.FromResult(ToDTO(result));
            }
            #endregion
        }

        class QueryObject<TOutDTO, TModel> : Query<TOutDTO, TModel>
            #region TYPE CONSTRINTS
    where TOutDTO : IModel
    where TModel : class, ISelfModel<TModel>, new()
            //-:cnd:noEmit
#if (!MODEL_USEDTO)
    , TOutDTO
#endif
            //+:cnd:noEmit
            #endregion
        {
            List<TModel> models;

            #region CONSTRUCTORS
            public QueryObject(List<TModel>? _models = null, bool initialzeData = false)
            {
                models = _models ?? new List<TModel>();
                if (!initialzeData)
                    return;
                IEnumerable<TModel>? items = GetInitialData();
                if (items == null)
                    return;
                models.AddRange(items);
            }
            #endregion

            #region ITEMS
            protected override IEnumerable<TModel> GetItems() => models;
            #endregion

            #region GET MODEL COUNT
            public override int GetModelCount() => models.Count;
            #endregion
        }
#endif
        //+:cnd:noEmit
        #endregion
    }
    #endregion
}
