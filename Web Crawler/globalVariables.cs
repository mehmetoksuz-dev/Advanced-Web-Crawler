using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Web_Crawler
{
    class globalVariables
    {
        public static object _obj_dicCrawlUrls_Lock = new object(); //locklarin içinde kullanacagiz cunku bazi durumlarda ikiden fazla thread çalışabilir.
        public static Dictionary<string, crawled_Urls> dicCrawlUrls = new Dictionary<string, crawled_Urls>();
        public static List<Task> lstRunTask = new List<Task>();
        public static int irMaxTaskCount=20; // maks çalışacak task sayısı
        public static int irMaxRetryCounter = 3; // max deneme sayısı
        public static int irMaxWaitHours = 24;

        public static HashSet<string> hashCrawledUrls = new HashSet<string>();
        public static HashSet<string> hashNewUrls = new HashSet<string>();
        public static HashSet<string> hashCurrentlyCrawlUrls = new HashSet<string>();

        public static bool dbControl = false;
    }
}
