/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
Credit for the code below goes to: https://avarnon.medium.com/how-to-show-enums-as-strings-in-swashbuckle-aspnetcore-628d0cc271e6 
*/

//-:cnd:noEmit
#if !TDD && MODEL_USESWAGGER
using System.Runtime.Serialization;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace MicroService.Common.API
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        void ISchemaFilter.Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                foreach (string enumName in Enum.GetNames(context.Type))
                {
                    System.Reflection.MemberInfo? memberInfo = context.Type.GetMember(enumName).FirstOrDefault(m => m.DeclaringType == context.Type);
                    EnumMemberAttribute? enumMemberAttribute = null;
                    if (memberInfo != null)
                    {
                        enumMemberAttribute = memberInfo?.GetCustomAttributes(typeof(EnumMemberAttribute), false).OfType<EnumMemberAttribute>().FirstOrDefault();
                    }
                    string label = string.IsNullOrWhiteSpace(enumMemberAttribute?.Value)
                     ? enumName
                     : enumMemberAttribute.Value;
                    model.Enum.Add(new OpenApiString(label));
                }
            }
        }
    }
}
#endif
//+:cnd:noEmit
