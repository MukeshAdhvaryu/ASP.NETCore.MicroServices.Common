/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using MicroService.Common.Interfaces;

namespace MicroService.Common.Parameters
{
    public readonly struct ObjParameter: IParameter
    {
        public ObjParameter(object value, string name)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public object Value { get; }
    }
}
