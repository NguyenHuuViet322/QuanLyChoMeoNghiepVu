using ESD.Utility.LogUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ESD.Utility
{
    public class TreeModel<T>
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public long Parent { get; set; }
        public string ParentPath { get; set; }
        public bool IsCheck { get; set; }
        public int Level { get; set; }
        public int Weight { get; set; }
        public int SortOrder { get; set; } = 0;
        public T Item { get; set; }
    }

    public static class Utils
    {
        public static Dictionary<int, string> GetDescribes<T>()
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            try
            {
                string[] names = Enum.GetNames(typeof(T));
                string[] array = names;
                foreach (string text in array)
                {
                    dictionary.Add((int)Enum.Parse(typeof(T), text), GetDescribe<T>(text));
                }
            }
            catch
            {
            }

            return dictionary;
        }
        public static string GetDescribe<T>(object name)
        {
            try
            {
                Type typeFromHandle = typeof(T);
                MemberInfo[] member = typeFromHandle.GetMember(name.ToString());
                object[] customAttributes = member[0].GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
                return ((DescriptionAttribute)customAttributes[0]).Description;
            }
            catch
            {
                return null;
            }
        }
        public static string GetValueDataLookup(string inputLookUp, string column)
        {
            try
            {
                var json = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(inputLookUp);
                if (Utils.IsNotEmpty(json) && json.FirstOrDefault().ContainsKey(column.ToUpper()))
                {
                    return json.FirstOrDefault()[column].ToString();
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        public static string GetValueDataLookups(string inputLookUp, params string[] columns)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<List<Hashtable>>(inputLookUp);
                if (IsNotEmpty(data))
                {
                    foreach (var column in columns)
                    {
                        var iData = data.FirstOrDefault();
                        if (iData.ContainsKey(column.ToUpper()))
                            return GetString(iData, column.ToUpper());
                    }
                }
            }
            catch
            {
            }
            return "";
        }
        public static T Deserialize<T>(string json) where T : new()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return new T();
            }
        }
        public static string Serialize<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return null;
            }
        }
        public static string SerializeHasTable(Hashtable obj)
        {
            try
            {
                var result = new Dictionary<string, string>();
                foreach (DictionaryEntry item in obj)
                {
                    var val = item.Value ?? "";
                    result.Add(item.Key.ToString(), val.ToString());
                }

                return Serialize(result);
            }
            catch
            {
                return null;
            }
        }
        public static string SerializeV2(dynamic data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                try
                {
                    return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
                }
                catch (Exception exx)
                {
                    return null;
                }
            }
        }
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return null;
            }
        }
        public static string Base64Encode(string plainText)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            }
            catch
            {
                return null;
            }
        }

        public static T Bind<T>(Hashtable data)
        {
            return ((T)Activator.CreateInstance(typeof(T))).Bind<T>(data);
        }
        public static T Bind<T>(this T obj, Hashtable data)
        {
            if (!object.Equals(data, null))
            {
                foreach (PropertyInfo info in obj.GetType().GetProperties())
                {
                    try
                    {
                        if (info.Name == "CreatedBy" || info.Name == "CreateDate" || info.Name == "UpdatedDate" || info.Name == "UpdatedBy")
                            continue;


                        DateTime? nullable;
                        Type propertyType = info.PropertyType;

                        if (data.ContainsKey(info.Name))
                        {
                            if (propertyType == typeof(string))
                            {
                                string str = GetString(data, info.Name);
                                info.SetValue(obj, str);
                            }
                            if (propertyType == typeof(string[]))
                            {
                                string[] strings = GetStrings(data, info.Name);
                                info.SetValue(obj, strings);
                            }
                            else if (propertyType == typeof(long))
                            {
                                long bigInt = GetBigInt(data, info.Name);
                                info.SetValue(obj, bigInt);
                            }
                            else if (propertyType == typeof(long?))
                            {
                                long? bigInt = GetBigIntNullable(data, info.Name);
                                info.SetValue(obj, bigInt);
                            }
                            else if ((propertyType == typeof(double)) || (propertyType == typeof(double?)))
                            {
                                double _double = GetDouble(data, info.Name);
                                info.SetValue(obj, _double);
                            }
                            else if (propertyType == typeof(double[]))
                            {
                                long[] bigInts = GetBigInts(data, info.Name);
                                info.SetValue(obj, bigInts);
                            }
                            else if (propertyType == typeof(long[]))
                            {
                                long[] bigInts = GetBigInts(data, info.Name);
                                info.SetValue(obj, bigInts);
                            }
                            else if (propertyType == typeof(int))
                            {
                                int @int = GetInt(data, info.Name);
                                info.SetValue(obj, @int);
                            }
                            else if (propertyType == typeof(int?))
                            {
                                int? @int = GetIntNullable(data, info.Name);
                                info.SetValue(obj, @int);
                            }
                            else if ((propertyType == typeof(byte)) || (propertyType == typeof(byte?)))
                            {
                                byte tinyint = GetTinyint(data, info.Name);
                                info.SetValue(obj, tinyint);
                            }
                            else if (propertyType == typeof(int[]))
                            {
                                int[] ints = GetInts(data, info.Name);
                                info.SetValue(obj, ints);
                            }
                            else if (propertyType == typeof(byte[]))
                            {
                                byte[] tinyints = GetTinyints(data, info.Name);
                                info.SetValue(obj, tinyints);
                            }
                            else if ((propertyType == typeof(bool)) || (propertyType == typeof(bool?)))
                            {
                                bool bol = GetBool(data, info.Name);
                                info.SetValue(obj, bol);
                            }
                            else if ((propertyType == typeof(DateTime)) || (propertyType == typeof(DateTime?)))
                            {
                                nullable = GetDatetime(data, info.Name, "dd-MM-yyyy HH:mm");
                                if (!object.Equals(nullable, null))
                                {
                                    DateTime? nullable2 = nullable;
                                    DateTime minValue = DateTime.MinValue;
                                    if (!(nullable2.HasValue ? (nullable2.GetValueOrDefault() <= minValue) : false))
                                    {
                                        DateTime? nullable3 = nullable;
                                        DateTime maxValue = DateTime.MaxValue;
                                        if (!(nullable3.HasValue ? (nullable3.GetValueOrDefault() >= maxValue) : false))
                                        {
                                            goto Label_0462;
                                        }
                                    }
                                }
                                info.SetValue(obj, null);
                            }
                        }
                        continue;
                    Label_0462:
                        info.SetValue(obj, nullable);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return obj;
        }

        public static long ByteToGb(long bytes)
        {
            if (bytes <= 0L)
            {
                return 0L;
            }
            return (((bytes / 0x400L) / 0x400L) / 0x400L);
        }

        public static T CopyTo<T>(object source)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
            }
            catch
            {
                return default(T);
            }
        }
        public static List<T> CopyTos<T>(dynamic sources)
        {
            try
            {
                var items = new List<T>();
                foreach (var source in sources)
                {
                    items.Add(Utils.CopyTo<T>(source));
                }
                return items;
            }
            catch (Exception ex)
            {
                return default(List<T>);
            }
        }

        public static long GetBigInt(Hashtable args, object i)
        {
            try
            {
                if (args.Count > 0)
                    return long.Parse(args[i].ToString());
            }
            catch
            {
            }
            return 0L;

        }
        public static long GetBigInt(string str)
        {
            try
            {
                long n = 0L;
                long.TryParse(str, out n);
                return n;
            }
            catch
            {
            }
            return 0L;

        }
        public static long? GetBigIntNullable(Hashtable args, object i)
        {
            try
            {
                if (args[i] == null)
                    return null;
                return long.Parse(args[i].ToString());
            }
            catch
            {
                return null;
            }
        }
        public static Double GetDouble(Hashtable args, object i)
        {
            try
            {
                if (args.Count > 0)
                    return Double.Parse(args[i].ToString());
            }
            catch
            {
            }
            return 0L;
        }
        public static Double[] GetDoubles(Hashtable args, object i)
        {
            try
            {
                List<double> strDb = new List<double>();
                var strs = GetStringsForDoubles(args, i);
                foreach (var item in strs)
                {
                    double dbl;
                    double.TryParse(item, out dbl);
                    strDb.Add(dbl);
                }
                return strDb.ToArray();
            }
            catch
            {
                return null;
            }
        }
        public static float GetFloat(Hashtable args, object i)
        {
            try
            {
                if (args.Count > 0)
                    return float.Parse(args[i].ToString().Replace(",", string.Empty));
            }
            catch
            {

            }
            return 0L;
        }
        public static float[] GetFloats(Hashtable args, object i)
        {
            try
            {
                if (args.Count > 0)
                {
                    var strs = GetStrings(args, i);
                    return strs.Select(t => float.Parse(t)).ToArray();
                }
            }
            catch
            {
            }
            return null;

        }
        public static decimal GetDecimal(Hashtable args, object i)
        {
            try
            {
                if (args.Count > 0)
                    return decimal.Parse(args[i].ToString());
            }
            catch
            {
            }
            return 0;

        }
        public static decimal[] GetDecimals(Hashtable args, object i)
        {
            try
            {
                var strs = GetStrings(args, i);
                return strs.Select(t => decimal.Parse(t)).ToArray();
            }
            catch
            {
                return null;
            }
        }
        public static long[] GetBigInts(Hashtable args, object i)
        {
            try
            {
                object obj2 = args[i];
                if (obj2.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                {
                    return JsonConvert.DeserializeObject<List<long>>(obj2.ToString()).ToArray();
                }
                if (obj2.GetType().ToString() == "System.String[]")
                {
                    var arrs = (string[])obj2;
                    return arrs.Select(t => long.Parse(t)).ToArray();
                }
                if (IsNumber(obj2.ToString()))
                {
                    return new long[] { long.Parse(obj2.ToString()) };
                }
                if (obj2.GetType().ToString() == "System.String")
                {
                    try
                    {
                        long[] objA = JsonConvert.DeserializeObject<List<long>>(obj2.ToString()).ToArray();
                        if (!object.Equals(objA, null) && objA.Any<long>())
                        {
                            return objA;
                        }
                    }
                    catch
                    {
                    }
                    return (from x in obj2.ToString().Split(new char[] { ',' }) select long.Parse(x)).ToArray<long>();
                }
                return (long[])obj2;
            }
            catch
            {
                return null;
            }
        }

        public static bool IsNumber(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            double n;
            bool isNumeric = double.TryParse(str, out n);
            return isNumeric;
        }
        public static bool IsNumberInt(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            } 
            bool isNumeric = int.TryParse(str, out int n);
            return isNumeric;
        }
        public static bool GetBool(Hashtable args, object i)
        {
            try
            {
                if (args[i].IsNotEmpty())
                    switch (args[i].ToString().Trim().ToLowerInvariant())
                    {
                        case "true":
                            return true;

                        case "t":
                            return true;

                        case "1":
                            return true;

                        case "yes":
                            return true;

                        case "y":
                            return true;

                        case "on":
                            return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime? GetDate(string dateString, string format = "dd/MM/yyyy")
        {
            try
            {
                if (format.IsEmpty())
                    format = "dd/MM/yyyy";

                return new DateTime?(DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture));
            }
            catch
            {
                return null;
            }
        }

        
        public static DateTime? ConvertToDateTime(object dt)
        {
            try
            {
                return (DateTime?)dt;
            }
            catch
            {
                return null;
            }
        }
        public static DateTime? GetDate(Hashtable args, object i, string format = "dd/MM/yyyy")
        {
            try
            {
                if (format.IsEmpty())
                    format = "dd/MM/yyyy";

                if ((args[i] is DateTime) || (args[i] is DateTime?))
                {
                    return new DateTime?((DateTime)args[i]);
                }
                if (args.Count > 0)
                    return new DateTime?(DateTime.ParseExact(args[i].ToString(), format, CultureInfo.InvariantCulture));
            }
            catch
            {
            }
            return null;
        }
        public static DateTime? GetDatetime(string dateString, string format = "dd/MM/yyyy HH:mm")
        {
            try
            {
                if (format.IsEmpty())
                    format = "dd/MM/yyyy HH:mm";

                return new DateTime?(DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture));
            }
            catch
            {
                return null;
            }
        }
        public static DateTime? GetDatetime(Hashtable args, object i, string format = "dd/MM/yyyy HH:mm")
        {
            try
            {
                if (format.IsEmpty())
                    format = "dd/MM/yyyy HH:mm";

                if ((args[i] is DateTime) || (args[i] is DateTime?))
                {
                    return new DateTime?((DateTime)args[i]);
                }
                if (args[i].IsNotEmpty())
                    return new DateTime?(DateTime.ParseExact(args[i].ToString(), format, CultureInfo.InvariantCulture));
            }
            catch
            {
            }
            return GetDate(GetString(args, i, false), "dd/MM/yyyy");

        }

        public static string GetString(Hashtable args, object i, bool removeSpace = false)
        {
            try
            {

                if (removeSpace)
                {
                    return args[i].ToString().Replace(" ", "").Trim();
                }
                if (args != null && args.Count > 0)
                {
                    if (args[i] != null)
                        return args[i].ToString().Trim(new char[] { ' ', '\t' });
                    return string.Empty;
                }
            }
            catch
            {
            }
            return string.Empty;

        }
        public static string GetFieldStrings<T>(string tableName, List<string> expts = null)
        {
            if (object.Equals(expts, null))
            {
                expts = new List<string>();
            }
            List<string> values = new List<string>();
            T local = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo info in local.GetType().GetProperties())
            {
                if (!expts.Contains(info.Name))
                {
                    values.Add(string.Format("[{0}].[{1}]", tableName, info.Name));
                }
            }
            return string.Join(", ", values);
        }

        private static string GetMd5(string text)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(text);
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider())
            {
                return provider.ComputeHash(bytes).Aggregate<byte, string>("", (current, x) => (current + string.Format("{0:x2}", x)));
            }
        }

        private static string GetSha1(string text)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(text);
            using (SHA1Managed managed = new SHA1Managed())
            {
                return managed.ComputeHash(bytes).Aggregate<byte, string>("", (current, x) => (current + string.Format("{0:x2}", x)));
            }
        }

        private static string GetSha256(string text)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(text);
            using (SHA256Managed managed = new SHA256Managed())
            {
                return managed.ComputeHash(bytes).Aggregate<byte, string>("", (current, x) => (current + string.Format("{0:x2}", x)));
            }
        }

        private static string GetSha512(string text)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(text);
            using (SHA512Managed managed = new SHA512Managed())
            {
                return managed.ComputeHash(bytes).Aggregate<byte, string>("", (current, x) => (current + string.Format("{0:x2}", x)));
            }
        }

        public static int GetInt(Hashtable args, object i)
        {
            try
            {
                if (args.Count > 0)
                    return int.Parse(args[i] != null ? args[i].ToString() : "0");
            }
            catch
            {
            }
            return 0;
        }
        public static int GetInt(string str)
        {
            int.TryParse(str, out int n);
            return n;
        }
        public static int? GetIntNullable(Hashtable args, object i)
        {
            try
            {
                if (args[i] == null) return null;
                return int.Parse(args[i].ToString());
            }
            catch
            {
                return null;
            }
        }
        public static int[] GetInts(List<string> parentStrs)
        {
            List<int> source = new List<int>();
            try
            {
                foreach (string str in parentStrs)
                {
                    string[] strArray = str.Trim(new char[] { '|' }).Split(new char[] { '|' });
                    source.AddRange((from x in strArray select int.Parse(x)).ToList<int>());
                }
            }
            catch
            {
            }
            return source.Distinct<int>().ToArray<int>();
        }

        public static int[] GetInts(Hashtable args, object i)
        {
            try
            {
                object obj2 = args[i];
                if (obj2 == null) return null;
                if (obj2.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                {
                    return JsonConvert.DeserializeObject<List<int>>(obj2.ToString()).ToArray();
                }
                if (obj2.GetType().ToString() == "System.String[]")
                {
                    var arrs = (string[])obj2;
                    return arrs.Select(t => int.Parse(t)).ToArray();
                }
                if (IsNumberInt(obj2.ToString()))
                {
                    return new int[] { int.Parse(obj2.ToString()) };
                }
                if (obj2.GetType().ToString() == "System.String")
                {
                    try
                    {
                        int[] objA = JsonConvert.DeserializeObject<List<int>>(obj2.ToString()).ToArray();
                        if (!object.Equals(objA, null) && objA.Any<int>())
                        {
                            return objA;
                        }
                    }
                    catch
                    {
                    }
                    return (from x in obj2.ToString().Split(new char[] { ',' }) select int.Parse(x)).ToArray<int>();
                }
                return (int[])obj2;
            }
            catch
            {
                return null;
            }
        }

        public static long? GetNulableBigInt(Hashtable args, object i)
        {
            try
            {
                if (string.IsNullOrEmpty(args[i].ToString()))
                {
                    return null;
                }
                return new long?(long.Parse(args[i].ToString()));
            }
            catch
            {
                return null;
            }
        }

        public static string JoinGroups(List<string> groups, string op = "OR")
        {
            if (groups.IsEmpty<string>())
            {
                return null;
            }
            op = string.Format(" {0} ", op);
            return string.Format("({0})", string.Join(op, groups));
        }

        public static Hashtable KeyValue<T>(this T obj)
        {
            Hashtable hashtable = new Hashtable();
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                hashtable.Add(info.Name, info.GetValue(obj, null));
            }
            return hashtable;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> data)
        {
            return ((data == null) || !data.Any<T>());
        }

        public static bool IsEmpty(object obj)
        {
            if (obj is string)
            {
                return string.IsNullOrEmpty((string)obj);
            }
            return object.Equals(obj, null);
        }
        public static bool IsNotEmpty<T>(this IEnumerable<T> data)
        {
            return ((data != null) && data.Any<T>());
        }

        public static long[] ParserBigInts(string parentStr, char c = '|')
        {
            List<long> source = new List<long>();
            try
            {
                string[] strArray = parentStr.Trim(new char[] { c }).Split(new char[] { c });
                source.AddRange((from x in strArray select long.Parse(x)).ToList<long>());
            }
            catch
            {
            }
            return source.Distinct<long>().ToArray<long>();
        }
        public static int[] ParserInts(string parentStr, char c = '|')
        {
            if(parentStr.IsEmpty())
                return new int[0];
            List<int> source = new List<int>();
            try
            {
                string[] strArray = parentStr.Trim(new char[] { c }).Split(new char[] { c });
                source.AddRange((from x in strArray select int.Parse(x)).ToList<int>());
            }
            catch
            {
            }
            return source.Distinct<int>().ToArray<int>();
        }

        public static string RenderOptions(dynamic lst, int selected = 0, bool all = true, string dfText = "", string dfVal = "0")
        {
            StringBuilder str = new StringBuilder();
            string option = "";
            if (all)
            {
                option = string.Format("<option data-target='{0}' selected data-url='{1}' value='{2}'>{3}</option>", "", "", dfVal, dfText);
                str.Append(option);
            }
            foreach (var item in lst)
            {
                long value = item.ID;
                string name = item.Name;

                if (value == selected)
                    option = string.Format("<option data-target='{0}' selected data-url='{1}' value='{2}'>{3}</option>", "", "", value, name);
                else
                    option = string.Format("<option data-target='{0}' data-url='{1}' value='{2}'>{3}</option>", "", "", value, name);
                str.Append(option);
            }
            return str.ToString();
        }
        public static bool TryGetValue(string[] args, int i, ref int result)
        {
            try
            {
                result = int.Parse(args.GetValue(i).ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool TryGetValue(string[] args, int i, ref long result)
        {
            try
            {
                result = long.Parse(args.GetValue(i).ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool TryGetValue(string[] args, int i, ref string result)
        {
            try
            {
                result = args.GetValue(i).ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static string UppercaseWords(string str)
        {
            List<string> values = new List<string>();
            try
            {
                foreach (string str2 in str.ToLowerInvariant().Split(new char[] { '-', ' ' }))
                {
                    char[] chArray = str2.ToCharArray();
                    chArray[0] = char.ToUpper(chArray[0]);
                    values.Add(new string(chArray));
                }
            }
            catch
            {
            }
            return string.Join(string.Empty, values);
        }
        public static string WSCleaner(string dataStr)
        {
            try
            {
                string str = Regex.Replace(Regex.Replace(Regex.Replace(dataStr, @"\s*\n\s*", "\n"), @"\s*\>\s*\<\s*", "><"), "<!--(.*?)-->", "");
                int index = str.IndexOf(">");
                if (index >= 0)
                {
                    str = str.Remove(index, 1).Insert(index, ">");
                }
                return str;
            }
            catch
            {
                return dataStr;
            }
        }
        public static string[] GetStrings(Hashtable args, object i, char separator)
        {
            try
            {
                object obj2 = args[i];
                return obj2.ToString().Split(new char[] { separator });
            }
            catch
            {
                return null;
            }
        }
        public static string[] GetStrings(Hashtable args, object i)
        {
            try
            {
                object obj2 = args[i];
                if (obj2 == null)
                    return null;
                if (obj2.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                {
                    return JsonConvert.DeserializeObject<List<string>>(obj2.ToString()).ToArray();
                }
                if (obj2.GetType().ToString() == "System.String")
                {
                    try
                    {
                        string[] objA = JsonConvert.DeserializeObject<List<string>>(obj2.ToString()).ToArray();
                        if (!object.Equals(objA, null) && objA.Any<string>())
                        {
                            return objA;
                        }
                    }
                    catch
                    {
                    }
                    return obj2.ToString().Split(new char[] { ',' });
                }
                return (string[])obj2;
            }
            catch
            {
                return null;
            }
        }
      
        public static string[] GetStrings(object obj2)
        {
            try
            {
                if (obj2 == null)
                    return null;
                if (obj2.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                {
                    return JsonConvert.DeserializeObject<List<string>>(obj2.ToString()).ToArray();
                }
                if (obj2.GetType().ToString() == "System.String")
                {
                    try
                    {
                        string[] objA = JsonConvert.DeserializeObject<List<string>>(obj2.ToString()).ToArray();
                        if (!object.Equals(objA, null) && objA.Any<string>())
                        {
                            return objA;
                        }
                    }
                    catch
                    {
                    }
                    return obj2.ToString().Split(new char[] { ',' });
                }
                return (string[])obj2;
            }
            catch
            {
                return null;
            }
        }
        public static string[] GetStringsForDoubles(Hashtable args, object i)
        {
            try
            {
                object obj2 = args[i];
                if (obj2.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                {
                    return JsonConvert.DeserializeObject<List<string>>(obj2.ToString()).ToArray();
                }
                if (obj2.GetType().ToString() == "System.String[]")
                {
                    return (string[])obj2;
                }
                if (obj2.GetType().ToString() == "System.String")
                {
                    try
                    {
                        string[] objA = JsonConvert.DeserializeObject<List<string>>(obj2.ToString()).ToArray();
                        if (!object.Equals(objA, null) && objA.Any<string>())
                        {
                            return objA;
                        }
                    }
                    catch
                    {
                    }
                    return new string[] { obj2.ToString() };
                }
                return (string[])obj2;
            }
            catch
            {
                return null;
            }
        }
        public static byte GetTinyint(Hashtable args, object i)
        {
            try
            {
                return byte.Parse(args[i].ToString());
            }
            catch
            {
                return 0;
            }
        }
        public static byte[] GetTinyints(Hashtable args, object i)
        {
            try
            {
                object obj2 = args[i];
                if (obj2.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                {
                    return JsonConvert.DeserializeObject<List<byte>>(obj2.ToString()).ToArray();
                }
                if (IsNumber(obj2.ToString()))
                {
                    return new byte[] { byte.Parse(obj2.ToString()) };
                }
                if (obj2.GetType().ToString() == "System.String")
                {
                    try
                    {
                        byte[] objA = JsonConvert.DeserializeObject<List<byte>>(obj2.ToString()).ToArray();
                        if (!object.Equals(objA, null) && objA.Any<byte>())
                        {
                            return objA;
                        }
                    }
                    catch
                    {
                    }
                    return (from x in obj2.ToString().Split(new char[] { ',' }) select byte.Parse(x)).ToArray<byte>();
                }
                return (byte[])obj2;
            }
            catch
            {
                return null;
            }
        }

        public static bool HasValue<T>(this T obj)
        {
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                try
                {
                    Type propertyType = info.PropertyType;
                    if ((propertyType == typeof(string)) && !string.IsNullOrEmpty(info.GetValue(obj).ToString()))
                    {
                        return true;
                    }
                    if (propertyType == typeof(string[]))
                    {
                        string[] data = (string[])info.GetValue(obj);
                        if (!data.IsEmpty<string>())
                        {
                            return true;
                        }
                    }
                    else if ((propertyType == typeof(long)) || (propertyType == typeof(long?)))
                    {
                        long num = (long)info.GetValue(obj);
                        if (num > 0L)
                        {
                            return true;
                        }
                    }
                    else if (propertyType == typeof(long[]))
                    {
                        long[] numArray = (long[])info.GetValue(obj);
                        if (!numArray.IsEmpty<long>())
                        {
                            return true;
                        }
                    }
                    else if ((propertyType == typeof(int)) || (propertyType == typeof(int?)))
                    {
                        int num2 = (int)info.GetValue(obj);
                        if (num2 > 0)
                        {
                            return true;
                        }
                    }
                    else if ((propertyType == typeof(byte)) || (propertyType == typeof(byte?)))
                    {
                        byte num3 = (byte)info.GetValue(obj);
                        if (num3 > 0)
                        {
                            return true;
                        }
                    }
                    else if (propertyType == typeof(int[]))
                    {
                        int[] numArray2 = (int[])info.GetValue(obj);
                        if (!numArray2.IsEmpty<int>())
                        {
                            return true;
                        }
                    }
                    else if (propertyType == typeof(byte[]))
                    {
                        byte[] buffer = (byte[])info.GetValue(obj);
                        if (!buffer.IsEmpty<byte>())
                        {
                            return true;
                        }
                    }
                    else if ((propertyType == typeof(bool)) || (propertyType == typeof(bool?)))
                    {
                        if ((bool)info.GetValue(obj))
                        {
                            return true;
                        }
                    }
                    else if ((propertyType == typeof(DateTime)) || (propertyType == typeof(DateTime?)))
                    {
                        DateTime time = (DateTime)info.GetValue(obj);
                        if (IsDate(new DateTime?(time)))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        public static bool IsDate(DateTime? dt)
        {
            if (!dt.HasValue)
            {
                return false;
            }
            DateTime? nullable = dt;
            DateTime minValue = DateTime.MinValue;
            if (nullable.HasValue)
            {
                return (nullable.GetValueOrDefault() != minValue);
            }
            return true;
        }

        public static bool IsObjDate(object dt, out DateTime? dateTime)
        {
            dateTime = dt as DateTime?;
            return dateTime != null;
        }

        public static string Summary(string dataString, int wordCount = 50)
        {
            if (string.IsNullOrEmpty(dataString))
            {
                return dataString;
            }
            int num = 0;
            string str = dataString.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
            {
                if (char.IsWhiteSpace(ch))
                {
                    num++;
                    if (num <= wordCount)
                    {
                        builder.Append(ch);
                        continue;
                    }
                    builder.Append(ch);
                    builder.Append('.');
                    builder.Append('.');
                    builder.Append('.');
                    break;
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static string Summarychar(string dataString, int charCount = 50)
        {
            if (string.IsNullOrEmpty(dataString))
            {
                return dataString;
            }
            int num = 0;
            string str = dataString.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
            {
                num++;
                if (num <= charCount)
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(ch);
                    builder.Append('.');
                    builder.Append('.');
                    builder.Append('.');
                    break;
                }
            }
            return builder.ToString();
        }


        public static string SummaryName(string dataString, int beforeWC = 2, int afterWC = 2)
        {
            if (string.IsNullOrEmpty(dataString))
            {
                return dataString;
            }
            string str = dataString.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();
            List<string> source = new List<string>();
            foreach (char ch in str)
            {
                if (char.IsWhiteSpace(ch))
                {
                    source.Add(builder.ToString());
                    builder = new StringBuilder();
                }
                else
                {
                    builder.Append(ch);
                }
            }
            source.Add(builder.ToString());
            try
            {
                int count = source.Count - afterWC;
                if (count < beforeWC)
                {
                    count = beforeWC + 1;
                }
                string str2 = string.Join(" ", source.Take<string>(beforeWC).ToList<string>());
                string str3 = string.Join(" ", source.Skip<string>(count).Take<string>(afterWC).ToList<string>());
                return string.Join(" ... ", new string[] { str2, str3 });
            }
            catch
            {
                return dataString;
            }
        }
        public static string TrimEnd(this string strSource, string strSuffix, bool ignoreCase = false)
        {
            try
            {
                if (object.Equals(strSource, null))
                {
                    return strSource;
                }
                if (object.Equals(strSuffix, null))
                {
                    return strSource;
                }
                if (!ignoreCase)
                {
                    if (!strSource.EndsWith(strSuffix))
                    {
                        return strSource;
                    }
                }
                else
                {
                    string str = strSource.ToLowerInvariant();
                    string str2 = strSuffix.ToLowerInvariant();
                    if (!str.EndsWith(str2))
                    {
                        return strSource;
                    }
                }
                return strSource.Substring(0, strSource.Length - strSuffix.Length);
            }
            catch
            {
                return strSource;
            }
        }



        public static string StringAlias(params object[] paramStrings)
        {
            if (paramStrings.Length == 0)
            {
                return null;
            }
            string str = string.Join("-", paramStrings);
            if (str.Length > 200)
            {
                decimal num = 200M;
                List<object> values = new List<object>();
                int charCount = (int)Math.Floor((decimal)(num / paramStrings.Length));
                foreach (object obj2 in paramStrings)
                {
                    values.Add(Summarychar(obj2.ToString(), charCount));
                }
                str = string.Join<object>("-", values);
            }
            bool flag = true;
            string str2 = str.Normalize(NormalizationForm.FormD);
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str2)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    if (char.IsLetterOrDigit(ch))
                    {
                        flag = true;
                        builder.Append(ch);
                    }
                    else if (flag)
                    {
                        flag = false;
                        builder.Append('-');
                    }
                }
            }
            return builder.Replace('Đ', 'D').Replace('đ', 'd').ToString().Normalize(NormalizationForm.FormD).Trim(new char[] { '-' });
        }


        public static int GetIDFromAlias(string id)
        {
            try
            {
                if (id == null)
                    return 0;
                return GetInt(id.Split("-").LastOrDefault());
            }
            catch (Exception) { }
            return 0;
        }


        public static long GetLongIDFromAlias(string id)
        {
            try
            {
                if (id == null)
                    return 0;
                return GetBigInt(id.Split("-").LastOrDefault());
            }
            catch (Exception) { }
            return 0;
        }


        public static bool IsNotEmpty(this object value)
        {
            if (value == null)
                return false;
            if (value.GetType() == typeof(string))
            {
                return !string.IsNullOrEmpty(value.ToString());
            }
            else if (value.GetType() == typeof(DateTime?))
            {
                return value.HasValue();
            }
            else if (value.GetType() == typeof(IList))
            {
                var list = (IList)value;
                if (list.Count == 0)
                    return false;
            }
            return true;
        }



        public static string Serialize(object obj)
        {
            return obj == null ? "" : JsonConvert.SerializeObject(obj);
        }

        public static string DateToString(DateTime? datetime, string format = "dd/MM/yyyy")
        {
            if (format.IsEmpty())
                format = "dd/MM/yyyy";
            if (datetime != null)
                return datetime.Value.ToString(format);
            return "";
        }
        public static string DateToString(DateTime datetime, string format = "dd/MM/yyyy")
        {
            if (format.IsEmpty())
                format = "dd/MM/yyyy";
            return datetime.ToString(format);
        }
        public static string Formatmoney(long obj)
        {
            return obj.ToString("0,0");
        }
        public static string Formatmoney(double obj)
        {
            return obj.ToString("0,0");
        }
        public static string Formatmoney(int obj)
        {
            return obj.ToString("0,0");
        }
        public static string Formatmoney(float obj)
        {
            return obj.ToString("0,0");
        }
        #region Collection Helpers
        public static TSource FirstOrNewObj<TSource>(this IEnumerable<TSource> source) where TSource : new()
        {
            try
            {
                if (source != null)
                {
                    if (source is IList<TSource> list)
                    {
                        if (list.Count > 0) return list[0];
                    }
                    else
                    {
                        using (IEnumerator<TSource> e = source.GetEnumerator())
                        {
                            if (e.MoveNext()) return e.Current;
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return new TSource();

        }
        public static TSource FirstOrNewObj<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) where TSource : new()
        {
            try
            {
                if (source != null && predicate != null)
                {
                    foreach (TSource element in source)
                    {
                        if (predicate(element)) return element;
                    }
                }
            }
            catch (Exception e)
            {
            }
            return new TSource();
        }

        #endregion

        public static List<TreeModel<T>> RenderTree<T>(List<TreeModel<T>> trees, TreeModel<T> parent, string str = "--")
        {
            int order = 1;
            var oTreeChilds = new List<TreeModel<T>>();
            if (parent == null) //Nếu không có parent thì lấy tất cả, trong trường hợp search
            {
                oTreeChilds = trees.OrderBy(t => t.ParentPath.Count(i => i == '|')).ThenBy(n => n.Name).ToList();
            }
            else if (parent.ID == 0) //cha là gốc
                oTreeChilds = trees.Where(x => x.Parent == parent.ID && x.ID > 0).ToList();
            else
                oTreeChilds =
                    trees.Where(
                        t =>
                            t.ParentPath == (parent.ParentPath + "|" + parent.ID) ||
                            t.ParentPath.IndexOf(parent.ParentPath + "|" + parent.ID + "|") == 0).ToList();
            var resultTrees = new List<TreeModel<T>>();
            if (IsEmpty(oTreeChilds))
            {
                return new List<TreeModel<T>>();
            }
            oTreeChilds = oTreeChilds.OrderBy(t => t.ParentPath.Count(i => i == '|')).ThenBy(n => n.Name).ToList();
            foreach (var item in oTreeChilds)
            {
                order++;
                if (item.IsCheck)
                    continue;
                if (parent == null || parent.ID == 0)
                {
                    item.Level = 0;
                    item.Weight = order;
                }
                else
                {
                    item.Level = parent.Level + 1;
                    item.Weight = parent.Weight * 1000 + order;
                }
                item.IsCheck = true;
                var temp = CopyTo<TreeModel<T>>(item);
                temp.Name = AddString(item.Level, str) + item.Name;
                resultTrees.Add(temp);
                resultTrees.AddRange(RenderTree(trees, item, str));
            }
            return resultTrees;
        }


        public static string AddString(int level, string str)
        {
            var newStr = string.Empty;
            if (level > 0)
            {
                for (int i = 0; i < level; i++)
                {
                    newStr += str;
                }
            }
            return newStr;
        }

        public static Dictionary<int, string> EnumToDic<T>()
        {
            var dic = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));
            foreach (var item in values)
            {
                string description;
                try
                {
                    description = ((DescriptionAttribute)item.GetType().GetMember(item.ToString()).FirstOrDefault()
                    .GetCustomAttribute(typeof(DescriptionAttribute))).Description;
                }
                catch (Exception ex)
                {
                    description = item.ToString();
                }
                dic.Add((int)item, description);

            }
            return dic;
        }
        public static Dictionary<int, string> EnumToValueDic<T>()
        {
            var dic = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));
            foreach (var item in values)
            {
                dic.Add((int)item, item.ToString());
            }
            return dic;
        }
        /// <summary>
        /// Lấy ra mô tả của Enum dựa vào key
        /// </summary>
        /// <typeparam name="T">Enum đầu vào</typeparam>
        /// <param name="key">key đầu vào</param>
        /// <returns></returns>
        public static string GetDescriptionEnumByKey<T>(int key)
        {
            var dic = new Dictionary<int, string>();
            var values = Enum.GetValues(typeof(T));
            foreach (var item in values)
            {
                string description;
                try
                {
                    description = ((DescriptionAttribute)item.GetType().GetMember(item.ToString()).FirstOrDefault()
                    .GetCustomAttribute(typeof(DescriptionAttribute))).Description;
                }
                catch (Exception ex)
                {
                    description = item.ToString();
                }
                dic.Add((int)item, description);
                if ((int)item == key)
                    return description;
            }
            return "";
        }

        public static string GetDescriptionEnum<T>(object name)
        {
            try
            {
                var type = typeof(T);
                var memInfo = type.GetMember(name.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                return ((DescriptionAttribute)attributes[0]).Description;
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, string> EnumToStringDic<T>()
        {
            var dic = new Dictionary<string, string>();
            var values = Enum.GetValues(typeof(T));
            foreach (var item in values)
            {
                string description;
                try
                {
                    description = ((DescriptionAttribute)item.GetType().GetMember(item.ToString()).FirstOrDefault()
                    .GetCustomAttribute(typeof(DescriptionAttribute))).Description;
                }
                catch (Exception ex)
                {
                    description = item.ToString();
                }
                dic.Add(item.ToString(), description);
            }
            return dic;
        }
        public static string MoneyDisplay(this float? money)
        {
            if (!money.HasValue)
                return string.Empty;
            return money.Value.ToString("N0", new CultureInfo("en-US"));
        }


        public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.ActionDescriptor.ActionName;
            }

            controller.ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = viewEngine.FindView(controller.ControllerContext, viewName, !partial);

                if (viewResult.Success == false)
                {
                    return $"A view with the name {viewName} could not be found";
                }

                ViewContext viewContext = new ViewContext(
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }


        public static List<int> ToListInt(this List<string> arr)
        {
            try
            {
                if (arr.IsEmpty())
                    return null;

                return arr.Select(n => GetInt(n)).ToList();
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static string GetProfileLang(Dictionary<int, string> dict, string lang)
        {
            try
            {
                var idLangs = new List<int>();
                if (lang.IsNotEmpty() && lang != "null")
                {
                    if (IsNumber(lang))
                        idLangs.Add(GetInt(lang));
                    else
                        idLangs = Deserialize<List<string>>(lang).ToListInt();
                }

                if (idLangs.IsNotEmpty())
                {
                    var langs = dict.Where(n => idLangs.Contains(n.Key) && n.Value.IsNotEmpty()).Select(n => n.Value).ToList();
                    if (langs.IsNotEmpty())
                    {
                        return string.Join(", ", langs);
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        public static string ConvertDataTabletoString(DataTable dt, List<string> columns = null)
        {
            if (dt == null || dt.Rows.Count == 0)
                return "";
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    if (columns != null && !columns.Exists(t => t.ToUpper() == col.ColumnName.ToUpper()))
                        continue;
                    //DacPV To Fix tạm cho chạy
                    if (col.ColumnName.ToUpper() == "DanhMucCha_DATALOOKUP" || col.ColumnName.ToUpper() == "THAMCHIEUDANHMUC_DATALOOKUP")
                        continue;
                    if (col.DataType == typeof(DateTime))
                    {
                        var val = Utils.DateToString((dr[col] == DBNull.Value ? null : (DateTime?)dr[col]));
                        row.Add(col.ColumnName, val);
                    }
                    else
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                }
                rows.Add(row);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(rows);
            ////DacPV Tranh anh huong
            //if(columns != null)
            //return Newtonsoft.Json.JsonConvert.SerializeObject(rows);
            //else
            //    return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }
        public static string TrimStart(this string target, string trimChars)
        {
            return target.TrimStart(trimChars.ToCharArray());
        }
        public static string TrimEnd(this string target, string trimChars)
        {
            return target.TrimEnd(trimChars.ToCharArray());
        }


        public static string ToOracleStringDatetime(this DateTime dt)
        {
            try
            {
                return $"TO_DATE('{dt.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
            }
            catch { }
            return "NULL";
        }
        public static string ToOracleDataLookup(this DataTable dt, List<string> columns = null)
        {
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    var json = ConvertDataTabletoString(dt, columns);
                    if (json.IsNotEmpty())
                    {
                        json = json.Replace("'", "''");
                    }
                    return $"'{json}'";
                }
            }
            catch { }
            return "NULL";
        }
        public static string ToOracleStringDatetime(this DateTime? dt)
        {
            if (!dt.HasValue)
                return "NULL";
            return ToOracleStringDatetime(dt.Value);
        }
        public static string ToOracleString(this string str)
        {
            try
            {
                if (str.IsNotEmpty())
                {
                    str = str.Replace("'", "''");
                }
                return $"N'{str}'";
            }
            catch
            {

            }
            return "''";
        }
        public static string ToOracleStringNumber(this int str)
        {
            return $"'{str}'";
        }


        public static int GetInt(DataTable dt, int index, string key)
        {
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    var val = dt.Rows[index][key].ToString();
                    return Convert.ToInt32(val);
                }
            }
            catch { }
            return 0;
        }

        public static string GetString(DataTable dt, int index, string key)
        {
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[index][key].ToString();
                }
            }
            catch { }
            return null;
        }
        public static IEnumerable<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("endDate must be greater than or equal to startDate");

            while (startDate <= endDate)
            {
                yield return startDate;
                startDate = startDate.AddDays(1);
            }
        }
        /// <summary>
        /// Lấy ra ngày đầu tiên trong tháng có chứa 
        /// 1 ngày bất kỳ được truyền vào
        /// </summary>
        /// <param name="dtDate">Ngày nhập vào</param>
        /// <returns>Ngày đầu tiên trong tháng</returns>
        public static DateTime GetFirstDayOfMonth(DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        /// <summary>
        /// Lấy ra ngày đầu tiên trong tháng được truyền vào 
        /// là 1 số nguyên từ 1 đến 12
        /// </summary>
        /// <param name="iMonth">Thứ tự của tháng trong năm</param>
        /// <returns>Ngày đầu tiên trong tháng</returns>
        public static DateTime GetFirstDayOfMonth(int iMonth)
        {
            DateTime dtResult = new DateTime(DateTime.Now.Year, iMonth, 1);
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        /// <summary>
        /// Lấy ra ngày cuối cùng trong tháng có chứa 
        /// 1 ngày bất kỳ được truyền vào
        /// </summary>
        /// <param name="dtInput">Ngày nhập vào</param>
        /// <returns>Ngày cuối cùng trong tháng</returns>
        public static DateTime GetLastDayOfMonth(DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }
        /// <summary>
        /// Lấy ra ngày cuối cùng trong tháng được truyền vào
        /// là 1 số nguyên từ 1 đến 12
        /// </summary>
        /// <param name="iMonth"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(int iMonth)
        {
            DateTime dtResult = new DateTime(DateTime.Now.Year, iMonth, 1);
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }
        
        public static string GetExtension(string path)
        {
            if (path == null)
                return null;

            return Path.GetExtension(path).ToLower().Substring(1);
        }


        
        public static IEnumerable<SelectListItem> ToSelectList(this Dictionary<int,string> dic, params int[] selecteds)
        {
            if (dic == null)
                return null;

            return dic.Select(n => new SelectListItem
            {
                Text = n.Value.ToString(),
                Value = n.Key.ToString(),
                Selected = selecteds.IsNotEmpty() && selecteds.Contains(n.Key)
            }).ToList();
        }

        public static string ToRoman(this int number)
        {
            if ((number < 0) || (number > 3999))
                return string.Empty;
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return string.Empty;
        }

    }
}
