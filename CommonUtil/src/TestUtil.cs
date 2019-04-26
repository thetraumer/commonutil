using System;
using System.Threading.Tasks;

namespace CommonUtil
{
    public abstract class TestUtil {
        private static Random random = new Random();
        private const string charsString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		
        public static bool IsTimeDiffSmall(DateTime refTime, DateTime checkedTime, long epsilonMs) {
            long elapsedMs = (refTime.Ticks - checkedTime.Ticks) / TimeSpan.TicksPerMillisecond;
            return Math.Abs(elapsedMs) <= epsilonMs;
        }

        public static string GetRandomString(int length) {
            var result = "";
            for (int i = 0; i < length; ++i)
                result += charsString[random.Next(charsString.Length)];
            return result;
        }
        
        public static void Pause(TimeSpan timeout) {
            Task.Delay((int) timeout.TotalMilliseconds).Wait();
        }

        public static void Pause(int timeoutMs) {
            Pause(TimeSpan.FromMilliseconds(timeoutMs));
        }
    }
}
