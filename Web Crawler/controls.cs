using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Web_Crawler
{
    class controls
    {
        public static bool urlControl(string url) //regex kontrol fonksiyonlarım
        {
            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
        }

        public static bool onlyNumber(string input)
        {
            string pattern = @"^[0-9]*$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(input);
        }
    }
}
