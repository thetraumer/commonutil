using System;

namespace CommonUtil {
    public class ThreadUtil {
        public static void ThrowInnerException(AggregateException ex) {
            ex.Handle((innerEx) => {
                if (innerEx is AggregateException) {
                    ThrowInnerException((AggregateException) innerEx);
                    return false;
                } else
                    throw innerEx;
            });
        }
    }
}
