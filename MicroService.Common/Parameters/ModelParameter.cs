/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/
using System.Collections;

using MicroService.Common.Interfaces;
using MicroService.Common.Parameters;

namespace MicroService.Common
{
    public interface IModelParameter : IParameter, IReadOnlyList<string>
    {
        /// <summary>
        /// Gets the first value if this object contains collection of values.
        /// </summary>
        string FirstValue { get; }
    }

    #region MODEL PARAMETER
    /// <summary>
    /// Represents a store which contains values of type T.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public readonly struct ModelParameter : IModelParameter
    {
        readonly IReadOnlyList<string> Items;

        public ModelParameter(IReadOnlyList<string> values, string _name)
        {
            Items = values;
            Name = _name;
        }
        public ModelParameter(string _name)
        {
            Items = new string[0];
            Name = _name;
        }

        public int Count => Items.Count;
        public string this[int index] => Items[index];

        public string Name { get; }

        public IEnumerator<string> GetEnumerator() =>
            Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        object IParameter.Value => Items;

        string IParameter.Name => Name;

        string IModelParameter.FirstValue => Items.Count> 0? Items[0]: null;
    }
    #endregion
}
