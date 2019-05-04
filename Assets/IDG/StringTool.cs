using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
namespace IDG
{
    public static class StringTool
    {
        public static string MD5(string value)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string GetGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString().Replace("-", "").ToUpper();
        }
        public static bool IsUsername(string str, int minLength = 6, int maxLength = 25)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(str, "^[A-Za-z0-9]+$") && str.Length >= minLength && str.Length <= maxLength)
            {
                return true;
            }
            return false;
        }
        public static bool IsEmail(string str)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(str, @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string GetCheckCode()
        {
            return GetGuid().Substring(0, 6);
        }
    }
}
