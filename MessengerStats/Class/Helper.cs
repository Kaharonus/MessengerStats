using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    public class Helper {
        /// <summary>
        /// Converts unix timestamp in miliseconds to DateTime format
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime ConvertTimeStamp(double unixTimeStamp) {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
    static class Extensions {
        /// <summary>
        /// Decodes Latin1 encoded text back to UTF8
        /// </summary>
        /// <param name="text">Latin1 encoded string</param>
        /// <returns>empty string if param is null or orig if exception was thrown while parsing</returns>
        public static string DecodeString(this string text) {
            if (text == null) {
                return "";
            }
            try {
                Encoding targetEncoding = Encoding.GetEncoding("ISO-8859-1");
                var unescapeText = System.Text.RegularExpressions.Regex.Unescape(text);
                var res = Encoding.UTF8.GetString(targetEncoding.GetBytes(unescapeText));
                return res;
            } catch (ArgumentException) {
                return text;
            }
        }
    }

}
