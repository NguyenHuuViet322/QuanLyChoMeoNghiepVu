using DAS.CodeGenerator.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DAS.CodeGenerator.Helper
{
    public static class Utils
    {
         
        /// <summary>
        /// Convert datatype từ db về c#
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static string ConvertDataType(string dataType, out int? maxLenght)
        {
            maxLenght = null;
            if (dataType == null)
                return string.Empty;

            dataType = dataType.ToLower();
            try
            {
                if (dataType.Contains("char") || dataType.Contains("text") || dataType.Contains("clob"))
                {
                    try
                    {
                        var math = Regex.Match(dataType, "\\((.*?)\\)");
                        maxLenght = TryParseNullable(math.Value.TrimStart('(').TrimEnd(')'));
                    }
                    catch (Exception) { }

                    return "string";
                }
                else if (dataType.Contains("datetime"))
                {
                    return "DateTime";
                }
                else if (dataType.Contains("bigint"))
                {
                    return "long";
                }
                else if (dataType.Contains("number") || dataType.Contains("int"))
                {
                    return "int";
                }
                else if (dataType.Contains("decimal") || dataType.Contains("float") )
                {
                    return "double";
                }
                else if (dataType.Contains("bit"))
                {
                    return "bool";
                }
                else if (dataType.Contains("tinyint"))
                {
                    return "tinyint";
                }
                else if (dataType.Contains("smallint"))
                {
                    return "short";
                }
                else  
                {
                    return "string";
                }

            }
            catch (Exception ex)
            {
            }

            return dataType;
        }

        public static int? TryParseNullable(string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? (int?)outValue : null;

        }

        public static string FirstCharToLowerCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
        }

        public static bool EqualIgnoreCase(this string str1, string str2)
        {
            if (str1 == null && str2 == null)
                return true;

            if (str1 == null || str2 == null)
                return false;

            return str1.Trim().ToLower() == str2.Trim().ToLower();
        }

        public static bool ContainsIgnoreCase(this string str1, string str2)
        {
            if (str1 == null && str2 == null)
                return true;

            if (str1 == null || str2 == null)
                return false;

            return str1.Trim().ToLower().Contains(str2.Trim().ToLower());
        }

        /// <summary>
        /// Lấy setting 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringAppSettings(string key)
        {
            if (key == null)
                return string.Empty;

            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(projectPath)
                   .AddJsonFile("appsettings.json")
                   .Build();

            return configuration.GetSection(key).Value;

        }

        public static List<TbColumn> GetTableColumns(string data)
        {
            var rs = new List<TbColumn>();
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var rows = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int index = 0; index < rows.Length; index++)
                    {
                        var row = rows[index];
                        try
                        {
                            var cols = row.Split(new[] { "\t" }, StringSplitOptions.None);
                            var name = cols.ElementAtOrDefault(0);
                            var strNullable = cols.ElementAtOrDefault(3)?.ToLower();
                            var dataType = cols.ElementAtOrDefault(2);
                            var title = cols.ElementAtOrDefault(1);
                            var isNullable = !string.IsNullOrEmpty(strNullable) && (strNullable == "true" || strNullable == "x" || strNullable == "1");
                            int? maxLen = null;
                            var tbColumn = new TbColumn()
                            {
                                Name = name,
                                Title = title?.ToUpper() == "NULL" ? string.Empty : title,
                                DataType = Utils.ConvertDataType(dataType, out maxLen),
                                Nullable = isNullable,
                                MaxLength = maxLen
                            };
                            rs.Add(tbColumn);
                        }
                        catch (Exception ex)
                        {
                            //AddLog($"GetTableColumns: Row {index} {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return rs; ;
        }

    }
}
