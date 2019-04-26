using System;
using System.Collections.Generic;

namespace CommonUtil {
    public static class ForEachExtension {
        public static void ForEach<T>(this IEnumerable<T> seq, Action<T> action) {
            foreach (var item in seq)
                action(item);
        }
    }
}
