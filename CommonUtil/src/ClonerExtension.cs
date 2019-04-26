using System;
using Newtonsoft.Json;

namespace CommonUtil {
    public static class ClonerExtension {
        public static T JsonClone<T>(this T source) {
            if (Object.ReferenceEquals(source, null)) {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var deserialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(deserialized, deserializeSettings);
        }
    }
}
