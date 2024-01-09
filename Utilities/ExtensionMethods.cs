using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public static class ExtensionMethods
    {
        public static bool IsNullorEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string TrimgHtmlTag(this string plainText)
        {
            return Regex.Replace(plainText, "<.*?>", String.Empty);
        }

        public static string ReplaceBr(this string plainText)
        {
            return plainText.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
        }
       
    }
}
