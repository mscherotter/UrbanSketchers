using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanSketchers
{
    public static class CollectionExtensions
    {
        public static void SetRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            collection.Clear();

            if (range == null)
            {
                return;
            }

            range.ToList().ForEach(collection.Add);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            if (range == null)
            {
                return;
            }

            range.ToList().ForEach(collection.Add);
        }

    }
}
