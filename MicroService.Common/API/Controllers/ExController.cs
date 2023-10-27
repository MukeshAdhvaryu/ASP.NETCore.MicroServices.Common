/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MicroService.Common.API
{
    #region IExController 
    /// <summary>
    /// This interface represents a contract of operations.
    internal interface IExController
    { }
    #endregion

    /// <summary>
    /// Generic controller. You must inherit this controller in your microservice project.
    /// We are using repository pattern so, we need a service repository
    /// to divert contract calls to it to perform contracted operations.
    /// </summary>
    /// </summary>
    /// <typeparam name="TInDTO">Type of input DTO.</typeparam>
    [ApiController]
    [Route("[controller]")]
    public abstract class ExController<TInDTO, TModel> : ControllerBase, IExController
        #region TYPE CONSTRAINTS
        where TInDTO : IModel, new()
        where TModel : class, ISelfModel<TModel>,
        new()
        #endregion
    {
        protected static readonly Type DTOType = typeof(TInDTO);
        static readonly TModel DummyModel = new TModel();
        #region GET MODEL
        protected static TModel GetModel(bool dummy) =>
           dummy? DummyModel: new TModel();
        #endregion

        #region PARSE TO DTO ENUMERABLE
        protected static IEnumerable<TInDTO?>? ToInDTOEnumerable(string query)
        {
            return Globals.ParseArray<TInDTO>(query);
        }
        #endregion

        #region PARSE TO DTO ENUMERABLE
        protected static TInDTO? ToInDTO(string query)
        {
            return Globals.Parse<TInDTO>(query);
        }
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit

