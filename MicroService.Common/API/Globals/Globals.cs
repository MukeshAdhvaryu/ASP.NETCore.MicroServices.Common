using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
using MicroService.Common.API;
using MicroService.Common.Models;
//-:cnd:noEmit
#if MODEL_SEARCHABLE
using MicroService.Common.Parameters;
#endif
//+:cnd:noEmit

using Microsoft.AspNetCore.Mvc;

namespace MicroService.Common
{
    partial class Globals
    {
        #region VARIABLES
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreCase;
        #endregion

        #region GET MODEL INSTANCE
        /// <summary>
        /// Creates model instance getiing 3rd generic parameter from specified controller type.
        /// </summary>
        /// <param name="controllerType">Type of controller which derives from generic Controller<,,> class</param>
        /// <returns>Retuns an empty instance of model.</returns>
        /// <exception cref="NotSupportedException">Thrown when incompitible controller type or incompitent controller type is supplied.</exception>
        public static IModel GetModel(this Type controllerType, bool dummyModel = false)
        {
            var baseType = GetRootControllerType(controllerType);
            object? model = baseType.GetMethod("GetModel", flags)?.Invoke(null, new object[] { dummyModel });
            if (model == null || !(model is IExModel))
                throw new NotSupportedException("Model is not supported!");
            return (IExModel)model;
        }
        #endregion

        #region GET CONTROLLER TYPE
        /// <summary>
        /// Creates model instance getiing 3rd generic parameter from specified controller type.
        /// </summary>
        /// <param name="controllerType">Type of controller which derives from generic Controller<,,> class</param>
        /// <param name="modelType">Type of mmodel created.</param>
        /// <returns>Retuns an empty instance of model.</returns>
        /// <exception cref="NotSupportedException">Thrown when incompitible controller type or incompitent controller type is supplied.</exception>
        public static Type? GetControllerType(this Type controllerType, out Type? dtoType)
        {
            var baseType = GetRootControllerType(controllerType);
            dtoType = baseType?.GetField("DTOType", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as Type;
            return baseType;
        }
        #endregion

        #region GET CONTROLLER TYPE
        static Type GetRootControllerType(Type controllerType)
        {
            if (controllerType == null || !controllerType.IsAssignableTo(typeof(IExController)))
                throw new NotSupportedException("Controller: " + controllerType?.Name + " is not supported!");
            Type baseType = controllerType;

            var topType = typeof(ControllerBase);
            Type? top = controllerType;
            while (top != null)
            {
                if (top.BaseType == topType)
                    break;
                top = top.BaseType;
            }
            if (top != null)
                baseType = top;
            return baseType;
        }
        #endregion

        #region TO DTO
        internal static object? ToDTO(this Type objType, string? json, Type controllerType)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            bool isEnumerable = objType.IsAssignableTo(typeof(IEnumerable));
            string methodname = isEnumerable ? "ToInDTOEnumerable" : "ToInDTO";
            return controllerType.GetMethod(methodname, flags)?.Invoke(null, new object[] { json });
        }
        #endregion

        #region TO DTO
        //-:cnd:noEmit
#if MODEL_SEARCHABLE
        internal static object? ToSearchParam(this Type objType, string? json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            bool isEnumerable = objType.IsAssignableTo(typeof(IEnumerable));
            if (isEnumerable)
                return ParseArray<SearchParameter>(json);
            else
                return Parse<SearchParameter>(json);
        }
#endif
        //+:cnd:noEmit

        #endregion
    }
}
#endif
//+:cnd:noEmit

