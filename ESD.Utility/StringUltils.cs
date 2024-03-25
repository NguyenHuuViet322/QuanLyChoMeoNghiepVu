using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ESD.Utility
{
    public static class StringUltils
    {
        /// <summary>
        /// MD5 encryption
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Md5Encryption(string text)
        {
            var md5Hasher = new MD5CryptoServiceProvider();
            byte[] bs = Encoding.UTF8.GetBytes(text);
            bs = md5Hasher.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        /// <summary>
        /// Generate random string
        /// </summary>
        /// <param name="numberOfString"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int numberOfString)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, numberOfString)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Check a string is an email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                       @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                       @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return emailRegex.IsMatch(email);
        }

        /// <summary>
        /// Convert a string to MetaTitle
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUnsignString(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }

            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            input = input.Replace(".", "-");
            input = input.Replace(" ", "-");
            input = input.Replace(",", "-");
            input = input.Replace(";", "-");
            input = input.Replace(":", "-");
            input = input.Replace("  ", "-");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            while (str2.Contains("--"))
            {
                str2 = str2.Replace("--", "-").ToLower();
            }
            return str2;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static Dictionary<int, string> GetEnumDictionary<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T is not an Enum type");

            return Enum.GetValues(typeof(T))
                .Cast<object>()
                .ToDictionary(k => (int)k, v => GetEnumDescription((Enum)v));
        }
        /// <summary>
        /// Có dấu sang không dâu
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToUnSign(this string str)
        {
            if (str == null)
                return str;
            string[] vietnameseSigns = new string[]
            {

                "aAeEoOuUiIdDyY",

                "áàạảãâấầậẩẫăắằặẳẵ",

                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

                "éèẹẻẽêếềệểễ",

                "ÉÈẸẺẼÊẾỀỆỂỄ",

                "óòọỏõôốồộổỗơớờợởỡ",

                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

                "úùụủũưứừựửữ",

                "ÚÙỤỦŨƯỨỪỰỬỮ",

                "íìịỉĩ",

                "ÍÌỊỈĨ",

                "đ",

                "Đ",

                "ýỳỵỷỹ",

                "ÝỲỴỶỸ"

            };
            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                    str = str.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
            }

            Regex reg = new Regex("[/*'\",_&#^@]");
            return reg.Replace(str, " ");
             
        }
        /// <summary>
        /// Có dấu sang không dâu (Kiểu camel)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertToCamelString(this string str)
        {
            if (str == null)
                return str;

            var arr = str.ConvertToUnSign().Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length == 1)
                return arr.FirstOrDefault();

           var newStr = string.Empty;
            foreach (var item in arr)
            {
                //Viết hoa chũ đầu
                newStr += item.Length == 1
                    ? item.ToUpper()
                    : (item.Substring(0, 1).ToUpper() + item.Substring(1));
            }
            return newStr;
        }
    }
}
