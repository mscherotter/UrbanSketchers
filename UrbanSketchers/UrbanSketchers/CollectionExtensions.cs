using System.Collections.Generic;
using System.Linq;

namespace UrbanSketchers
{
    /// <summary>
    ///     <see cref="ICollection{T}" /> extensions
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Sets the items in a collection to a range, clearing it first.
        /// </summary>
        /// <typeparam name="T">the type of item in the collection</typeparam>
        /// <param name="collection">the destination collection</param>
        /// <param name="range">the range to add to the collection</param>
        public static void SetRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            lock (collection)
            {
                collection.Clear();

                if (range == null)
                    return;

                range.ToList().ForEach(collection.Add);
            }
        }

        /// <summary>
        ///     Adds the items in the range to the collection
        /// </summary>
        /// <typeparam name="T">the type of item in the collection</typeparam>
        /// <param name="collection">the destination collection</param>
        /// <param name="range">the range to add to the collection</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            lock (collection)
            {
                if (range == null)
                    return;

                range.ToList().ForEach(collection.Add);
            }
        }
    }
}