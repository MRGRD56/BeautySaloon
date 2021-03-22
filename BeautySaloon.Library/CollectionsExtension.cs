using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautySaloon.Library
{
    public static class CollectionsExtension
    {
        public static T TryGet<T>(IEnumerable<T> collection, int index)
        {
            if (index < 0)
            {
                throw new ArgumentException("TryGet: index cannot be < 0");
            }
            if (collection.Count() - 1 >= index)
            {
                return collection.ElementAt(index);
            }

            return default;
        }
    }
}
