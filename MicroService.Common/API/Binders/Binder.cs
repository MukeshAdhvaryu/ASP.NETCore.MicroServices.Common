/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit
using MicroService.Common.Models;

using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MicroService.Common.API
{
    public abstract class Binder : IModelBinder
    {
        #region VARIABLES
        protected readonly IObjectModelValidator Validator;
        #endregion

        #region CONSTRUCTORS
        protected Binder(IObjectModelValidator _validator)
        {
            Validator = _validator;
        }
        #endregion

        #region BIND MODEL
        public abstract Task BindModelAsync(ModelBindingContext bindingContext);
        #endregion
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
