/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

using MicroService.Common.Attributes;
using MicroService.Common.Exceptions;
using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common
{
    public static partial class Globals
    {
        #region VARIABLES
        public static readonly JsonSerializerOptions JsonSerializerOptions;
        static HashSet<string> Names;
        static readonly object GlobalLock = new object();
        internal static readonly string Url;
        static volatile bool isProductionEnvironment;
        #endregion

        #region CONSTRUCTOR
        static Globals()
        {
            lock (GlobalLock)
            {
                JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web).AddDefaultOptions();
                Names = new HashSet<string>(3);
                Url = @"/{0} /{1}";
            }
        }
        #endregion

        #region PROPERTIES
        public static bool IsProductionEnvironment {get => isProductionEnvironment; internal set { isProductionEnvironment = value; } }
        #endregion

        #region ADD DEFAULT JSON OPTIONS
        public static JsonSerializerOptions AddDefaultOptions(this JsonSerializerOptions JsonSerializerOptions)
        {
            JsonSerializerOptions.IncludeFields = true;
            JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            JsonSerializerOptions.IgnoreReadOnlyFields = true;
            JsonSerializerOptions.IgnoreReadOnlyProperties = true;
            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            return JsonSerializerOptions;
        }
        #endregion

        #region GET CLEAN NAME
        static string GetCleanName(Type type)
        {
            //Let's check if a specific name for the associated controller is provided as an attribute of TModel.
            //If provided, no do further, assign name and exit.
            var nameAttribute = type.GetCustomAttribute<ModelAttribute>();
            if (!string.IsNullOrEmpty(nameAttribute?.Name))
                return nameAttribute.Name;

            var name = type.Name;

            //If TModel is an interface remove 'I' suffix from the name.
            //if (type.IsInterface && name.Length > 1 && char.IsUpper(name[1]) && (name[0] == 'I' || name[0] == 'i'))
            //    name = name.Substring(1);

            if (name.Length == 1 || !type.IsGenericType)
            {
                return name;
            }
            //If TModel is genereic type, remove part representing generic name for example: name of Any<T> resolves in Any `1 as name of the type.
            var idx = name.IndexOf('`');
            if (idx != -1)
                name = name.Substring(0, idx);
            return name;
        }
        #endregion

        #region GET NAME
        public static string GetName(this Type contractType, Type modelType)
        {
            string finalName;
            var modelAttr = modelType.GetCustomAttribute<ModelAttribute>();
            finalName = GetCleanName(modelType);
            if (!string.IsNullOrEmpty(modelAttr?.Name) && !Names.Contains(modelAttr.Name))
                finalName = modelAttr.Name;
            if (string.IsNullOrEmpty(finalName))
                finalName = GetCleanName(modelType);

            int count = contractType.GenericTypeArguments.Length;
            if (count < 2)
                goto EXIT;
            int i = -1;
            while (Names.Contains(finalName))
            {
                ++i;
                if (i == 2)
                    continue;
                if (i >= count)
                    break;
                modelType = contractType.GenericTypeArguments[i];
                modelAttr = modelType.GetCustomAttribute<ModelAttribute>();
                finalName = GetCleanName(modelType);
                if (!string.IsNullOrEmpty(modelAttr?.Name) && !Names.Contains(modelAttr.Name))
                    finalName = modelAttr.Name;
            }
            EXIT:
            Names.Add(finalName);
            return finalName;
        }
        #endregion

        #region PARSE AS T
        public static T? Parse<T>(string query)
            where T: new()
        {
            try
            {
                return  JsonSerializer.Deserialize<T>(query, JsonSerializerOptions);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region PARSE AS OBJECT
        public static object? Parse(string query, Type type)
        {
            try
            {
                return JsonSerializer.Deserialize(query, type, JsonSerializerOptions);
            }
            catch
            {
                throw;
            }
        }
        #region PARSE AS OBJECT
        public static T? Parse<T>(JsonElement element)
        {
            try
            {
                var obj = JsonSerializer.Deserialize(element, typeof(T), JsonSerializerOptions);
                if (obj == null)
                    return default(T);
                return (T)obj;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #endregion

        #region PARSE ENUMERABLE
        public static T?[] ParseArray<T>(string query)
            where T : new()
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(query)))
                {
                    return JsonSerializer.DeserializeAsyncEnumerable<T>(stream, JsonSerializerOptions).ToBlockingEnumerable().ToArray();
                }
            }
            catch
            {
                throw;
            }
        }

        public static U[] ParseArray<T, U>(string query)
            where T : U, new()
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(query)))
                {
                    return JsonSerializer.DeserializeAsyncEnumerable<T>(stream, JsonSerializerOptions).ToBlockingEnumerable().OfType<U>().ToArray();
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region GET APPROPRIATE EXCEPTION
        /// <summary>
        /// Supplies an appropriate exception for a failure in a specified method.
        /// </summary>
        /// <param name="exceptionType">Type of exception to get.</param>
        /// <param name="additionalInfo">Additional information to aid the task of exception supply.</param>
        /// <returns>Instance of SpecialException class.</returns>
        internal static ModelException GetModelException(this IExModelExceptionSupplier model, ExceptionType exceptionType, string? additionalInfo = null, Exception? innerException = null)
        {
            var message = model.GetModelExceptionMessage(exceptionType, additionalInfo);
            return exceptionType.Create(message, innerException);
        }
        #endregion

        #region CREATE MODEL EXCEPTION
        /// <summary>
        /// Creates an instance of ModelException.
        /// </summary>
        /// <param name="message">Custom message provided by user.</param>
        /// <param name="type">Type of the model exception.</param>
        /// <param name="exception">Original exception raised by some operation performed on model.</param>
        /// <returns></returns>
        public static ModelException Create(this ExceptionType type, string message, Exception? exception = null)
        {
            if (exception == null)
                return new ModelException(message, type);
            return new ModelException(message, type, exception);
        }
        #endregion
    }
}
