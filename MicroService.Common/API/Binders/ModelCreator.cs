/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
using System.Reflection;

using MicroService.Common.Web.API;

using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common.Models
{
    /// <summary>
    /// This class creates model instance on calling: GetModel method.
    /// </summary>
    public abstract class ModelCreator
    {
        /// <summary>
        /// Creates model instance getiing 3rd generic parameter from specified controller type.
        /// </summary>
        /// <param name="controllerType">Type of controller which derives from generic Controller<,,> class</param>
        /// <param name="modelType">Type of mmodel created.</param>
        /// <returns>Retuns an empty instance of model.</returns>
        /// <exception cref="NotSupportedException">Thrown when incompitible controller type or incompitent controller type is supplied.</exception>
        protected virtual IModel GetModel(Type controllerType, out Type modelType)
        {
            if (!controllerType.IsAssignableTo(typeof(IExController)))
                throw new NotSupportedException("Controller: " + controllerType.Name + " is not supported!");

            var baseType = typeof(ControllerBase);
            Type? top = controllerType;
            while (top != null)
            {
                if (top.BaseType == baseType)
                    break;
                top = top.BaseType;
            }
            if (top != null)
                controllerType = top;

            var model = controllerType.GetMethod("GetNewModel", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, null);
            if (model == null || !(model is IExModel))
                throw new NotSupportedException("Model is not supported!");

            modelType = model.GetType();
            return (IExModel) model;
        }
    }
}
#endif
//+:cnd:noEmit

