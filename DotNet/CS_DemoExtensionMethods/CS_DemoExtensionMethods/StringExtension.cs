using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoExtensionMethods
{
    internal static class StringExtension
    {
        public static string Capitalize(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            //return char.ToUpper(str[0]) + str.Substring(1);
            return str.ToUpper();
        }

        public static string Capitalize(this string str, int number)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            //return char.ToUpper(str[0]) + str.Substring(1);
            return str.Substring(0, Math.Min(number, str.Length)).ToUpper() + str.Substring(Math.Min(number, str.Length));
        }
    }
}
