using System;
using System.Collections.Generic;
using System.Text;

namespace Web_Crawler
{
    public class crawled_Urls
    {
        public string srUrl { get; set; }   //hashlerin içineki value'lar için kullandık
        public string srNormalizedUrl { get; set; }
        public string srKey { get; set; } //normal url'lerin hash edilen hali.

        public DateTime dtCrawlingStarted;
        public  DateTime dtCrawlingEnded;

        public string srRootSite {get; set;} //dictionary'e eklerken kullanılacak
        public string srCrawledSource { get; set; }

        public bool blCurrenCrawl = false;

        public bool blCrawledOrNot = false;

        public int irCrawlRetryCounter = 0;
    }
}
