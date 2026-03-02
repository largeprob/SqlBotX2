using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SqlBoTx.Net.Share.Security
{
    /// <summary>
    /// AES加密解密工具类
    /// 优点：跨平台，可在不同机器间迁移加密数据
    /// 缺点：需要安全地存储密钥和IV
    /// </summary>
    public static class AesEncryption
    {
        // AES密钥长度：128, 192, 256 bits
        private const int KeySize = 256;
        private const int BlockSize = 128;

        /// <summary>
        /// 生成随机的密钥和IV
        /// </summary>
        public static (string Key, string IV) GenerateKeyAndIV()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.GenerateKey();
                aes.GenerateIV();

                return (
                    Key: Convert.ToBase64String(aes.Key),
                    IV: Convert.ToBase64String(aes.IV)
                );
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="keyBase64">Base64编码的密钥</param>
        /// <param name="ivBase64">Base64编码的IV</param>
        public static string Encrypt(string plainText, string keyBase64, string ivBase64)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);

            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        public static string Decrypt(string encryptedText, string keyBase64, string ivBase64)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return encryptedText;

            byte[] key = Convert.FromBase64String(keyBase64);
            byte[] iv = Convert.FromBase64String(ivBase64);
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;
                aes.BlockSize = BlockSize;
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
