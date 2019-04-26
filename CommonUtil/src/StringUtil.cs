using System;
using System.Security.Cryptography;
using System.Text;

namespace CommonUtil {
    public abstract class StringUtil {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        public static byte[] Sha256(string input) {
            using(SHA256 sha256 = SHA256.Create()) {
                byte[] inputBytes = StringToBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return hashBytes;
            }
        }

        public static string ByteToLowerHex(byte[] bytes) {
            return BitConverter.ToString(bytes)
                .Replace("-", string.Empty)
                .ToLower();
        }

        private static int FindLowercaseEndIndex(string url) {
            int queryStartIndex = url.IndexOf("?");
            int pathStartIndex = url.IndexOf("/");
            int result = -1;
            if (queryStartIndex > 0)
                result = queryStartIndex;
            if (result > 0 && pathStartIndex > 0)
                result = Math.Min(result, pathStartIndex);
            else if (pathStartIndex > 0)
                result = pathStartIndex;
            return result;
        }

        public static string BytesToString(byte[] bytes) {
            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] StringToBytes(string str) {
            return Encoding.UTF8.GetBytes(str);
        }

        public static int TryParseInt(string intText) {
            int number;
            if (!Int32.TryParse(intText, out number))
                throw new ArgumentException("Invalid number format");
            return number;
        }

        public static string FormatDateTime(DateTime dateTime) {
            return dateTime.ToString("dd MMMM yyyy HH:mm");
        }

        public static string GetRandomString(int binaryLength) {
            byte[] rndBytes = new byte[binaryLength];
            rngCsp.GetBytes(rndBytes);
            return Convert.ToBase64String(rndBytes, Base64FormattingOptions.None);
        }
    }
}
