using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Web_Crawler.globalVariables;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Linq;
using static Web_Crawler.publicFunctions;
using HtmlAgilityPack;
using System.Web;
using System.Threading;
using System.Windows;
using static Web_Crawler.cs_fetch_result;

namespace Web_Crawler
{
    public static class publicFunctions
    {
        public static MainWindow refMainWin; //listboxa ulasmak icin

        private static StreamWriter logs = new StreamWriter("logs.txt", true); //istenmeyen linkler burada tutulacak

        private static StreamWriter swNewFoundUrls = new StreamWriter("new_urls_logs.txt", true); //url logları da burada

        private static object _obj_lock_logs = new object();
        private static object _obj_lock_swNewFoundUrls = new object();

        public static bool blCrawlStop = false;
        static publicFunctions() 
        {
            logs.AutoFlush = true; //arabelleği temizliyoruz
            swNewFoundUrls.AutoFlush = true;
        }

        private enum enLogType //enum içine 2 değişken tanımladık, write kısmında kullanıcaz
        {
            BypassedUrl,
            FoundUrl
        }

        private static void writeLogstoFile(string srLog, enLogType whichLog) // dosyalara yazdıracağız
        {
            switch (whichLog)
            {
                case enLogType.BypassedUrl:
                    lock (_obj_lock_logs)
                    {
                        logs.WriteLine(srLog + "\t" + DateTime.Now);
                    }
                    break;
                case enLogType.FoundUrl:
                    lock (_obj_lock_swNewFoundUrls)
                    {
                        swNewFoundUrls.WriteLine(srLog + "\t" + DateTime.Now);
                    }
                    break;
                default:
                    break;
            }

        }
        public static void loadCrawlingDic()
        {

            lock (_obj_dicCrawlUrls_Lock)
            {
                if (File.Exists("root_urls.txt")) // crawl edilmesini istedigimiz siteler
                {
                    foreach (var vrLine in File.ReadLines("root_urls.txt"))
                    {
                        addToDic(vrLine);
                    }
                }

                foreach (var vrUrls in dicCrawlUrls) //foreach ile dictionary içindeki urlleri alıyoruz
                {
                    if (vrUrls.Value.blCrawledOrNot == false)
                    {
                        if (vrUrls.Value.irCrawlRetryCounter >= irMaxRetryCounter) //3 kere denediyse
                        {
                            if (vrUrls.Value.dtCrawlingStarted.AddHours(irMaxWaitHours) > DateTime.Now) //24 saat yasaklıyoruz
                            {
                                continue;
                            }
                        }
                        hashNewUrls.Add(vrUrls.Value.srUrl);
                    }
                    else
                    {
                        hashCrawledUrls.Add(vrUrls.Value.srUrl); //devam ediyoruz
                    }

                }

            }


        }

        private static void addToDic(string srUrl)
        {
            lock (_obj_dicCrawlUrls_Lock)
            {
                crawled_Urls myUrl = new crawled_Urls();
                myUrl.srNormalizedUrl = srUrl.normalizeUrls(); // globalization yapıyoruz
                myUrl.srUrl = srUrl;
                myUrl.srKey = srUrl.hashUrlFunc(); //hashliyourz
                myUrl.srRootSite = srUrl.getRootUrl();
                if (!dicCrawlUrls.ContainsKey(myUrl.srKey))
                {
                    dicCrawlUrls.Add(myUrl.srKey, myUrl); // yok ise ekliyoruz
                }
            }

        }
        public static void saveCrawlingDic()
        {
            lock (_obj_dicCrawlUrls_Lock) //we used lock because of two threads will work or maybe more than two
            {
                string json = JsonConvert.SerializeObject(dicCrawlUrls, Formatting.Indented); //girintili json formatında tutabilmek için.
            }
        }

        public static void updateListStatusBox(string srMsg) //main screen'deki listbox güncellemesi için kullanılan kısım
        {
            refMainWin.Dispatcher.BeginInvoke(new Action(() =>
            {
                refMainWin.lstUrls.Items.Insert(0, $"{srMsg}\t{DateTime.Now}");
            }));
        }

        public static bool controlCrawlingCanStart() //task'in durumunu kontrol ediyoruz true döndürürse main'de crawl başlatabiliriz
        {
            lock (lstRunTask)
            {
                foreach (var vrTask in lstRunTask.ToList()) //liste yapmalıyız cunku diger degisikliklerde hata döndürebilir
                {
                    if (vrTask.Status == TaskStatus.RanToCompletion || vrTask.Status == TaskStatus.Canceled || vrTask.Status == TaskStatus.Faulted)
                    {
                        lstRunTask.Remove(vrTask); // hatalı ya da iptal durumunda listeden kaldır
                    }
                }
                if (irMaxTaskCount > lstRunTask.Count)
                    return true;
                return false;
            }
        }

        public static void crawlUrl(string srUrl)
        {
            updateListStatusBox("starting to crawl \t" + srUrl); //zaman ekledik //burası listboxa taranan siteleri çekiyor.
            addToDic(srUrl);
            int irRetryCount = 0;

            var vrUrlKey = srUrl.hashUrlFunc(); //Url'yi hashliyoruz
            lock (_obj_dicCrawlUrls_Lock)
            {
                var vrCurrent = dicCrawlUrls[vrUrlKey]; //obje oluşturduk ve buradan erişeceğiz.
                vrCurrent.blCurrenCrawl = true; // crawl başladı.
                vrCurrent.dtCrawlingStarted = DateTime.Now;
                vrCurrent.irCrawlRetryCounter++;
                irRetryCount = vrCurrent.irCrawlRetryCounter;
            }

            var vrFetchResults = page_fetcher.fetch_a_page(srUrl);


            if (vrFetchResults.fetchStatusCode == System.Net.HttpStatusCode.OK)
            {
                lock (_obj_dicCrawlUrls_Lock)
                {
                    var vrCurr = dicCrawlUrls[vrUrlKey];
                    vrCurr.blCurrenCrawl = false;
                    vrCurr.dtCrawlingEnded = DateTime.Now;
                    vrCurr.blCrawledOrNot = true;
                    vrCurr.srCrawledSource = vrFetchResults.srFetchSource;
                }

                lock (hashCrawledUrls)
                {
                    hashCrawledUrls.Add(srUrl); // hashcrawled url içine srUrl yi atıyoruz.
                }

                var vrNewUrls = returnNewUrls(vrFetchResults.srFetchSource, srUrl); //yeni urlleri var içine atıyoruz
                addNewUrlsToQueue(vrNewUrls); //kuyruğa ekliyor
            }

            else
            {
                if (irRetryCount < irMaxRetryCounter) //deneme sayısını yeniden kontrol ediyoruz 3ten küçükse ekliyor.
                {
                    lock (hashNewUrls)
                    {
                        hashNewUrls.Add(srUrl);
                    }
                }
            }
            lock (hashCurrentlyCrawlUrls)
            {
                hashCurrentlyCrawlUrls.Remove(srUrl); //diğer durumda siliyor
            }


            // veritabanı işlemleri
            try
            {
                var vrLinks = srUrl;
                var vrTitle = cs_fetch_result.srTitle;
                var vrRetryCount = irRetryCount;

                foreach (var vrItems in vrLinks.ToString())
                {
                    if (vrTitle == null || vrTitle=="")
                    {
                        vrTitle = "There is no title";
                    }
                    if (dbConnection.return_data_set("SELECT crawledlinks FROM tblCrawledLinks WHERE crawledlinks='" + vrLinks.ToString() + "'", out string msg).Tables[0].Rows.Count <= 0)
                    {
                        string srAddLinksQuery = "INSERT INTO tblCrawledLinks(crawledlinks, title, retrycount) values (@crawledlinks, @title, @retrycount)";
                        List<dbConnection.cmdParameterType> linksParam = new List<dbConnection.cmdParameterType>
                        {
                            new dbConnection.cmdParameterType("@crawledlinks",vrLinks.ToString()),
                            new dbConnection.cmdParameterType("@title", vrTitle.ToString()),
                            new dbConnection.cmdParameterType("@retrycount", vrRetryCount),
                        };

                        if (dbConnection.cmd_update_DB(srAddLinksQuery, linksParam) > 0)
                        {
                            globalVariables.dbControl = true;
                        }
                    }
                }
            }

            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }

        }

        private static void addNewUrlsToQueue(List<string> lstNewUrls) //linkleri liste olarak kuyruğa(sıraya) ekliyoruz
        {
            lock (hashCrawledUrls) // lock içinde yazalım thread bitmeden diğer threade geçmesini engelleyelim.
            {
                foreach (var vrUrl in lstNewUrls.ToList()) //burada listeliyoruz
                {
                    if (hashCrawledUrls.Contains(vrUrl)) //aynı link var mı? varsa kaldırıyoruz.
                        lstNewUrls.Remove(vrUrl);
                }

                foreach (var vrNewUrl in lstNewUrls) //dictionary'e urlyi ekliyoruz
                {
                    addToDic(vrNewUrl);
                }

                if (blCrawlStop == false) //crawl durmadığı sürece ekliyoruz.
                {
                    lock (hashNewUrls)
                    {
                        foreach (var vrNewUrl in lstNewUrls)
                        {
                            hashNewUrls.Add(vrNewUrl);
                        }
                    }
                }
            }
        }

        public static List<string> returnNewUrls(string sources, string crawledurls)
        {
            List<string> listFoundNewUrls = new List<string>();
            HtmlDocument hdDoc = new HtmlDocument();
            hdDoc.LoadHtml(sources); //htmldocument paketinden aldığımız veri

            var vrNodes = hdDoc.DocumentNode.SelectNodes("//a[@href]"); //linkler <a href> olarak başladığı için a[@href] kullandık

            if (vrNodes != null)
            {
                foreach (HtmlNode link in vrNodes)
                {
                    var vrHrefVal = link.Attributes["href"].Value.ToString(); //<a href> mantığı kullanıyoruz
                    var baseUrl = new Uri(crawledurls); //crawledurls parametresini baseUrl nin içinde Uri classından tanımladık

                    Uri newUrl; 

                    if (Uri.TryCreate(baseUrl, vrHrefVal, out newUrl)) //yeni url oluşturuyoruz
                    {
                        string srNewUrl = newUrl.AbsoluteUri.ToString().urlNormalize();
                        if (controlUrlCrawled(crawledurls, srNewUrl, false))
                        {
                            writeLogstoFile(srNewUrl, enLogType.FoundUrl); //dosyaya yazdırıyoruz
                            listFoundNewUrls.Add(srNewUrl);
                        }
                    }
                }
            }
            return listFoundNewUrls;
        }

        public class statistics { //istatistik classı içinde istatistiklerimizi tanımlıyoruz
            public int curCrawlUrlCount { get; set; }
            public int totalCrawlUrlCount { get; set; }
            public int totalUncrawledUrlCount { get; set; }
        }
        public static statistics returnStatistics() //burada döndürüyoruz
        {
            statistics myTempStatistics = new statistics();
            myTempStatistics.curCrawlUrlCount = hashCurrentlyCrawlUrls.Count; //su an crawl edilen urller 
            myTempStatistics.totalCrawlUrlCount = hashCrawledUrls.Count; //crawl edilenler
            myTempStatistics.totalUncrawledUrlCount = hashNewUrls.Count; //crawl edilmemisler
            return myTempStatistics;
        }


        private static string urlNormalize(this string srUrl)
        {
            // "#" kullanarak split ettik
            return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(srUrl)).Split('#').FirstOrDefault(); //URLnin kodunu çözer ve kodu çözülen dizeyi döndürür.
        }

        private static readonly List<string> listOfNotAllowed = new List<string>
        {
            ".pdf","jpg.",".jpeg",".png",".css",".js",".docx",".doc",".svg" //bu uzantıları engelledik çünkü dosya halinde geliyorlar
        };
        private static bool controlUrlCrawled(string crawledurls, string srNewUrls, bool blAllowExtUrls = true)
        {
            Uri orgUrl = new Uri(crawledurls);
            Uri newUrl = new Uri(srNewUrls);

            //sadece http ve https izin vermek için

            if (newUrl.Scheme != Uri.UriSchemeHttp && newUrl.Scheme != Uri.UriSchemeHttps) // güvenli alandan okuyoruz
            {
                writeLogstoFile($"scheme {newUrl.Scheme.ToString()} not allowed url : {srNewUrls}", enLogType.BypassedUrl);
                return false;
            }


            if (blAllowExtUrls == false)
            {
                if (orgUrl.Host.ToString() != newUrl.Host.ToString())
                {
                    writeLogstoFile($"external links are not allowed url : {srNewUrls}", enLogType.BypassedUrl);
                    return false;
                }
            }


            foreach (var vrEx in listOfNotAllowed)
            {
                if (srNewUrls.ToLowerInvariant().EndsWith(vrEx))
                {
                    writeLogstoFile($"extension {vrEx} not allowed url : {srNewUrls}", enLogType.BypassedUrl);
                    return false;
                }
            }

            return true;
        }
    }
}
