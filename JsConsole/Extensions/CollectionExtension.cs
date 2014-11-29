using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JsConsole.Extensions
{
    public static class CollectionExtension
    {
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> collection)
        {
            return ((collection == null) || !collection.Any<TSource>());
        }

        public static bool IsNullOrEmpty(this IEnumerable collection)
        {
            if (collection == null)
            {
                return true;
            }
            var is2 = collection as ICollection;
            if (is2 != null)
            {
                return (is2.Count == 0);
            }
            return !collection.GetEnumerator().MoveNext();
        }
    }
}