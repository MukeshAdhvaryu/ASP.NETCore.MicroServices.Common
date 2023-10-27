/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
//-:cnd:noEmit
#if !TDD
//+:cnd:noEmit

using MicroService.Common.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.Reflection;

namespace MicroService.Common.Models
{
    partial class Model<TModel> : IEntityTypeConfiguration<TModel>, IExModel
    {
        protected static BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;
        protected static string jsonArr = "[{0}]";

        protected virtual void Configure(EntityTypeBuilder<TModel> builder) { }
        void IEntityTypeConfiguration<TModel>.Configure(EntityTypeBuilder<TModel> builder)
        {
            Configure(builder);            
        }
    }
}
//-:cnd:noEmit
#endif
//+:cnd:noEmit
