using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ESD.Utility
{
    public class EnumUltils
    {
        public static bool HasFlag<T>(T operation, T checkflag) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            int op = Convert.ToInt32(operation);
            int check = Convert.ToInt32(checkflag);
            return (op & check) == check;
        }


        public static bool HasAllFlags<T>(T operation, params T[] checkflags) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            foreach (T checkflag in checkflags)
            {
                int op = Convert.ToInt32(operation);
                int check = Convert.ToInt32(checkflag);
                if ((op & check) != check)
                    return false;
            }
            return true;
        }

        public static bool HasFlag(int op, int check)
        {
            return (op & check) == check;
        }


        public static bool HasAllFlags(int op, params int[] checks)
        {
            foreach (int check in checks)
            {
                if ((op & check) != check)
                    return false;
            }
            return true;
        }

        public static Dictionary<T, string> GetDescription<T>() where T : Enum
        {
            var rs = new Dictionary<T, string>();
            // gets the Type that contains all the info required    
            // to manipulate this type    
            Type enumType = typeof(T);

            // I will get all values and iterate through them    
            var enumValues = enumType.GetEnumValues();

            foreach (T item in enumValues)
            {
                MemberInfo memberInfo = enumType.GetMember(item.ToString()).First();
                var descriptionAttribute = memberInfo.GetCustomAttribute<DescriptionAttribute>();
                if (descriptionAttribute != null)
                {
                    rs.Add(item, descriptionAttribute.Description);
                }
                else
                {
                    rs.Add(item, item.ToString());
                }
            }
            return rs;
        }
    }
}
