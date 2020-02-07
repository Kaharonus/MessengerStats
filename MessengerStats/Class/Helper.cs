using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerStats {
    class Helper {
    }
    static class Extensions {
        public static string DecodeString(this string text) {
            Encoding targetEncoding = Encoding.GetEncoding("ISO-8859-1");
            var unescapeText = System.Text.RegularExpressions.Regex.Unescape(text);
            var res = Encoding.UTF8.GetString(targetEncoding.GetBytes(unescapeText));
            return res;
        }
    }

}
