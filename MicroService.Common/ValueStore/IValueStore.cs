/* Licensed under the MIT/X11 license.
* This notice may not be removed from any source distribution.
 Author: Mukesh Adhvaryu.
*/

namespace MicroService.Common.Interfaces
{
    #region IValueStore<T>
    /// <summary>
    /// Represents a store which contains values of type T.
    /// </summary>
    /// <typeparam name="T">Any type.</typeparam>
    public interface IValueStore<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets name of this store.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the first value in this store.
        /// </summary>
        T FirstValue { get; }

        /// <summary>
        /// Indicates if this store is empty or not.
        /// </summary>
        bool IsEmpty { get; }
    }
    #endregion
}
