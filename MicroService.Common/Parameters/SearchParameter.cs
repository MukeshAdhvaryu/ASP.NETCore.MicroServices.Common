/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Parameters
{
    public interface ISearchParameter : IParameter
    {
        Criteria Criteria { get; }
        AndOr AndOr { get; }
    }

    public readonly struct SearchParameter: ISearchParameter
    {
        public SearchParameter(string name, object value, Criteria criteria = Criteria.Equal, AndOr andOr = AndOr.AND)
        {
            Name = name;
            Value = value;
            Criteria = criteria;
            AndOr = andOr;
        }
        public string Name { get; }
        public object Value { get; }
        public Criteria Criteria { get; }
        public AndOr AndOr { get; }
    }
}
