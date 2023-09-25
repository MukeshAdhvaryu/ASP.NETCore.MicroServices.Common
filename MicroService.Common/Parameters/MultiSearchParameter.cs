/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Models;

namespace MicroService.Common.Parameters
{
    /// <summary>
    /// Reprents a parameter which provides criteria for search and joining condition with other parameters.
    /// </summary>
    public interface IMultiSearchParameter : ISearchParameter
    {
        /// <summary>
        /// Gets joining condition with other parameters used for the search.
        /// </summary>
        AndOr AndOr { get; }
    }

    /// <summary>
    /// Reprents a parameter which provides criteria for search and joining condition with other parameters.
    /// </summary>
    public readonly struct MultiSearchParameter : IMultiSearchParameter
    {
        public static readonly new MultiSearchParameter Empty = new MultiSearchParameter();

        public MultiSearchParameter(string name, object value, Criteria criteria = Criteria.Equal, AndOr andOr = AndOr.AND)
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
