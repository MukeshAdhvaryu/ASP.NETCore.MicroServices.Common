/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
namespace MicroService.Common.Interfaces
{
    /// <summary>
    /// Represents an object which serves as a parameter to aid some operation.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets name of this object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets value of this object.
        /// </summary>
        object? Value { get; }
    }
}
