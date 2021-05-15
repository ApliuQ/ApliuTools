using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Apliu.Tools.PlatformTargetView
{
    public class FileMd5
    {
        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        public static string GetMD5(string filePath)
        {
            return GetMD5(File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        public static string GetMD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
