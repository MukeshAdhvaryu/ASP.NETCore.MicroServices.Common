/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

namespace MicroService.Common.Models
{
    /// <summary>
    /// Provides status of an operation.
    /// </summary>
    public enum ResultStatus : byte
    {
        /// <summary>
        /// Indicates that operation is failed.
        /// </summary>
        Failure,

        /// <summary>
        /// Indicates that operation is successful.
        /// </summary>
        Sucess,

        /// <summary>
        /// Indicates that operation is ignored for a specified call.
        /// </summary>
        Ignored,

        /// <summary>
        /// Indicates that operation  for a specified call could not be performed because of missing value.
        /// </summary>
        MissingValue,

        /// <summary>
        /// Indicates that operation for a specified call could not be performed because of missing value.
        /// And, the value is required to be supplied.
        /// </summary>
        MissingRequiredValue,
    }
}
namespace MicroService.Common.Services
{
    /// <summary>
    /// Provides Scope options for injecting service repository.
    /// </summary>
    public enum ServiceScope : byte
    {
        /// <summary>
        /// Injects scoped service.
        /// </summary>
        Scoped,

        /// <summary>
        /// Injects transient service.
        /// </summary>
        Transient,

        /// <summary>
        /// Injects singleton service.
        /// </summary>
        Singleton,
    }
}
