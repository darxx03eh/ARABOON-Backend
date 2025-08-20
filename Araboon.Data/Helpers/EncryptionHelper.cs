using System.Security.Cryptography;
using System.Text;

namespace Araboon.Data.Helpers
{
    public static class EncryptionHelper
    {
        private static String Key;
        public static void Initialize(String key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Encryption key is not set!");
            Key = key;
        }
        public static String Encrypt(String plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.GenerateIV();
            var iv = aes.IV;
            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            using var ms = new MemoryStream();
            ms.Write(iv, 0, iv.Length);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public static string Decrypt(string encrypted)
        {
            var bytes = Convert.FromBase64String(encrypted);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            var iv = bytes.Take(16).ToArray();
            var cipher = bytes.Skip(16).ToArray();
            using var decryptor = aes.CreateDecryptor(aes.Key, iv);
            using var ms = new MemoryStream(cipher);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
