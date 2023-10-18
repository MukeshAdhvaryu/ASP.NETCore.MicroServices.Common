/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Interfaces;

namespace MicroService.Common.Parameters
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

    public readonly struct ObjParameter: IParameter
    {
        public ObjParameter(object? value, string name)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public object? Value { get; }
    }
}
