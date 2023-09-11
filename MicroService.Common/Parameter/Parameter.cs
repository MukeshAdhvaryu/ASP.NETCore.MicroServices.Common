/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

namespace MicroService.Common.Parameters
{
    public interface IParameter
    {
        string Name { get; }
        object Value { get; }
    }

    public readonly struct Parameter: IParameter
    {
        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public object Value { get; }
    }
}
