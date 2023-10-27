/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE) ||(!MODEL_NONREADABLE || !MODEL_NONQUERYABLE)
using System.Runtime.CompilerServices;

using MicroService.Common.CQRS;
#endif
//+:cnd:noEmit

using MicroService.Common.Models;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace MicroService.Common.Interfaces
{
    #region IContract
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    public interface IContract : IModelCount 
    { }
    #endregion

    #region IContract<TOutDTO, TModel, TID>
    /// <summary>
    /// This interface represents a contract of operations.
    /// </summary>
    /// <typeparam name="TOutDTO">Interface representing the model.</typeparam>
    /// <typeparam name="TModel">Model of your choice.</typeparam>
    /// <typeparam name="TID">Primary key type of the model.</typeparam>
    public interface IContract<TOutDTO, TModel, TID> : IContract, IFirstModel<TModel>
        #region TYPE CONSTRINTS
        where TOutDTO : IModel, new()
        where TModel : class, ISelfModel<TID, TModel>,
        //-:cnd:noEmit
#if (!MODEL_USEDTO)
        TOutDTO,
#endif
        //+:cnd:noEmit
        new()
        where TID : struct
        #endregion
    {
        //-:cnd:noEmit
#if (!MODEL_NONREADABLE && !MODEL_NONQUERYABLE)
        IQuery<TOutDTO, TModel, TID> Query { get; }
#endif
#if (MODEL_APPENDABLE || MODEL_UPDATABLE || MODEL_DELETABLE)
        ICommand<TOutDTO, TModel, TID> Command { get; }
#endif
        //+:cnd:noEmit
    }
    #endregion
}
