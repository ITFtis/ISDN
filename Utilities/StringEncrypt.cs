using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public  class StringEncrypt
    {
        protected static readonly string keyText= "StringEncrypt";
        protected static readonly DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        static StringEncrypt() {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["EncryptKey"]) == false)
            {
                keyText = ConfigurationManager.AppSettings["EncryptKey"];
            };
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(keyText, md5.ComputeHash(Encoding.UTF8.GetBytes(keyText)));
            des.Key = rfc2898.GetBytes(des.KeySize / 8);
            des.IV = rfc2898.GetBytes(des.BlockSize / 8);
        }
        public static string encrypt(string plainText)
        {
            var dateByteArray = Encoding.UTF8.GetBytes(plainText);
            var sb = new StringBuilder();
            // 加密
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(dateByteArray, 0, dateByteArray.Length);
                cs.FlushFinalBlock();
                //return Convert.ToBase64String(ms.ToArray());
                foreach (byte b in ms.ToArray())
                {
                    sb.AppendFormat("{0:X2}", b);
                }
                return sb.ToString();
            }
        
        }
        public static string decrypt(string encrypted)
        {
            try
            {
                // var dateByteArray = Convert.FromBase64String(encrypted);
                 byte[] dataByteArray = new byte[encrypted.Length / 2];
                for (int x = 0; x < encrypted.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(encrypted.Substring(x * 2, 2), 16));
                    dataByteArray[x] = (byte)i;
                }
                // 解密
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception) {
                return encrypted;
            }
        
        }
    }
}
