using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AspNet3.AppLib.Tools
{
    // https://kenhaggerty.com/articles/article/aspnet-core-31-2fa-authenticating
    public static class TwoFactorAuth
    {
        public static string GetAuthenticatorKey()
        {
            // Generates a new base32 encoded 160-bit security secret (size of SHA1 hash).
            byte[] bytes = new byte[20];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return Base32.ToBase32(bytes);
        }

        public static int GetAuthenticatorCode(string key)
        {
            var unixTimestamp = (DateTime.UtcNow.Ticks - 621355968000000000L) / 10000000L;
            var window = unixTimestamp / (long)30;
            var keyBytes = Base32.FromBase32(key);
            var counter = BitConverter.GetBytes(window);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            var hmac = new HMACSHA1(keyBytes);
            var hash = hmac.ComputeHash(counter);
            var offset = hash[^1] & 0xf;

            // Convert the 4 bytes into an integer, ignoring the sign.
            var binary =
                ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | (hash[offset + 3]);

            return binary % (int)Math.Pow(10, 6);
        }

        public static long GetCurrentCounter()
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return 30 - (long)(DateTime.UtcNow - unixEpoch).TotalSeconds % 30;
        }
    }
}
