using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_System.Extensions
{
    using System.Globalization;
    using System.Text.RegularExpressions;


        public static class StringExtensions
        {
            public static string Titleize(this string text)
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text).ToSentenceCase();
            }

            public static string ToSentenceCase(this string str)
            {
                var result= Regex.Replace(str, "[a-z][A-Z]", 
                    m => m.Value[0] + " " + char.ToLower(m.Value[1]));
                return result;
            }
        }
   
}