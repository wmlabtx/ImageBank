using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ImageBank
{
    public static class HelperEncrypting
    {
        private static readonly byte[] SaltBytes = { 0xFF, 0x15, 0x20, 0xD5, 0x24, 0x1E, 0x12, 0xAA, 0xCC, 0xFF };
        private const int Interations = 1000;

        public static byte[] Decrypt(byte[] bytesToBeDecrypted, string password)
        {
            byte[] decryptedBytes = null;

            try
            {
                using (var ms = new MemoryStream())
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    var passwordBytes = Encoding.ASCII.GetBytes(password);
                    var key = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, Interations);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);
                    aes.Mode = CipherMode.CBC;
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Flush();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }
            catch (CryptographicException)
            {
            }

            return decryptedBytes;
        }

        public static byte[] Encrypt(byte[] bytesToBeEncrypted, string password)
        {
            byte[] encryptedBytes;

            using (var ms = new MemoryStream())
            using (var aes = new RijndaelManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                var passwordBytes = Encoding.ASCII.GetBytes(password);
                var key = new Rfc2898DeriveBytes(passwordBytes, SaltBytes, Interations);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cs.Flush();
                }

                encryptedBytes = ms.ToArray();
            }

            return encryptedBytes;
        }
    }
}
