/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using MicroService.Common.Attributes;
using MicroService.Common.Models;

namespace CQRS.Common
{
    public static class Globals
    {
        #region VARIABLES
        public static readonly JsonSerializerOptions JsonSerializerOptions;
        static HashSet<string> Names;
        static readonly object GlobalLock = new object();
        internal static readonly string Url;
        #endregion

        #region CONSTRUCTOR
        static Globals()
        {
            lock (GlobalLock)
            {
                JsonSerializerOptions = new JsonSerializerOptions().AddDefaultOptions();
                Names = new HashSet<string>(3);
                Url = @"/{0} /{1}";
            }
        }
        #endregion

        #region ADD DEFAULT JSON OPTIONS
        public static JsonSerializerOptions AddDefaultOptions(this JsonSerializerOptions JsonSerializerOptions)
        {
            JsonSerializerOptions.IncludeFields = true;
            JsonSerializerOptions.PropertyNameCaseInsensitive = true;
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

        #region TRY PARSE
        public static bool TryParse<T>(string query, out T? result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(query, JsonSerializerOptions);
                return true;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region TRY PARSE ENUMERABLE
        public static bool TryParseEnumerable<T>(string query, out IEnumerable<T?>? result)  
        {
            try
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(query)))
                {
                    result = JsonSerializer.DeserializeAsyncEnumerable<T>(stream, JsonSerializerOptions).ToBlockingEnumerable().ToArray();
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
