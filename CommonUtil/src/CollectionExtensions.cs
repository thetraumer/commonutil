using System;
using System.Collections.Generic;

namespace CommonUtil {
    public static class CollectionExtensions {
        public static void ForEach<T>(this IEnumerable<T> seq, Action<T> action) {
            foreach (var item in seq)
                action(item);
        }

        public static bool Exists<T>(this IEnumerable<T> seq, Predicate<T> match) {
            foreach (var item in seq)
                if (match(item))
                    return true;
            return false;
        }
    }
}
