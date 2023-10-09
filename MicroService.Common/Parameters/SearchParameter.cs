/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

using MicroService.Common.Interfaces;
using MicroService.Common.Models;

namespace MicroService.Common.Parameters
{
    /// <summary>
    /// Reprents a parameter which provides criteria for search.
    /// </summary>
    public interface ISearchParameter : IParameter
    {
        /// <summary>
        /// Gets criteria for the intended search.
        /// </summary>
        Criteria Criteria { get; }
    }

    /// <summary>
    /// Reprents a parameter which provides criteria for search.
    /// </summary>
    public readonly struct SearchParameter: ISearchParameter
    {
        public static readonly SearchParameter Empty = new SearchParameter();
        public SearchParameter(string name, object? value, Criteria criteria = Criteria.Equal)
        {
            Name = name;
            Value = value;
            Criteria = criteria;
        }
        public string Name { get; }
        public object? Value { get; }
        public Criteria Criteria { get; }
    }
}
