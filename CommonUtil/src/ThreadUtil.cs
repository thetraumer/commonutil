using System;

namespace CommonUtil {
    public class ThreadUtil {
        public static void ThrowInnerException(AggregateException ex) {
            ex.Handle((x) => {
                throw x;
            });
        }
    }
}
