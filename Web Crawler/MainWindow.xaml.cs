using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Threading;
using System.IO;
using System.Windows.Threading;
using static Web_Crawler.publicFunctions;
using static Web_Crawler.globalVariables;
using System.Text.RegularExpressions;
using static Web_Crawler.controls;

namespace Web_Crawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ThreadPool.SetMaxThreads(100000, 100000); //thread çalışacak iş parçacığı.
            ThreadPool.SetMinThreads(100000, 100000);
            publicFunctions.refMainWin = this;

            for (int i = 0; i < 4; i++) //başlangıçta istatistikleri döndürmem için
            {
                lstStatistics.Items.Add("statistics");
            }
        }

        DispatcherTimer dispatcherCrwl = new DispatcherTimer(); //timer başlattık

        private void updateAllStats(object sender, EventArgs e) //istatistikleri tuttugum fonksiyon
        {
            refMainWin.Dispatcher.BeginInvoke(new Action(() =>
            {
                var vrStats = publicFunctions.returnStatistics();
                var vrCurentlyCrawling = "Count of currently crawling urls: " + vrStats.curCrawlUrlCount.ToString("N0");
                var vrTotalCrawling = "Count of total crawled urls: " + vrStats.totalCrawlUrlCount.ToString("N0");
                var vrUnCrawled = "Count of waiting to crawl urls : " + vrStats.totalUncrawledUrlCount.ToString("N0");
                var vrAllCrawlCount = "Count of all links : " + (vrStats.totalUncrawledUrlCount + vrStats.totalCrawlUrlCount).ToString("N0");

                lstStatistics.Items[0] = vrCurentlyCrawling;
                lstStatistics.Items[1] = vrTotalCrawling;
                lstStatistics.Items[2] = vrUnCrawled;
                lstStatistics.Items[3] = vrAllCrawlCount;
            }));
        }
        private void btnStartCrawling_Click(object sender, RoutedEventArgs e) //crawlı başlattığımız alan, start buttonu.
        {
            publicFunctions.loadCrawlingDic(); 
            dispatcherCrwl.Tick += new EventHandler(doMainCrawl);
            dispatcherCrwl.Tick += new EventHandler(updateAllStats);
            dispatcherCrwl.Interval = new TimeSpan(0, 0, 0, 0, 500); //500ms ayarladık
            dispatcherCrwl.Start();
        }

        private void doMainCrawl(object sender, EventArgs e)
        {
            string srFile = "root_urls.txt"; //dosya işlemleri
            int irLinkcounter = 0;
            if (File.Exists(srFile))
            {
                using (StreamReader srRead = new StreamReader(srFile))
                {
                    while (srRead.Read() > 0)
                    {
                        srRead.ReadLine();
                        irLinkcounter++;
                    }
                    //srRead.Close();
                }
            }
            else { return; }

            if (irLinkcounter>irMaxTaskCount) //link task'ten fazla olursa engellemek için yazdığım kısım
            {
                MessageBox.Show($"You can crawl max : {irMaxTaskCount}");
                return;
            }

            for (int i = 0; i < irLinkcounter; i++) //linkcounter zaten task'ten fazla olamayacağı için max linkcountera kadar crawl edecek.
            {
                if (!controlCrawlingCanStart()) //task durumlarını kontrol ediyoruz
                {
                    return;
                }

                string srNewUrls = null;
                lock (hashNewUrls) //lock kullandım çünkü thread bitmeden diğer bir threada geçmesini istemiyoruz (bunun sebebi de birden fazla thread kullanmamız)
                {
                    if (hashNewUrls.Count == 0) //yeni url yok ise tamamlamaya geçiyor.
                    {
                        lock (hashCurrentlyCrawlUrls)
                        {
                            if (hashCurrentlyCrawlUrls.Count == 0) //şu anda crawl'da olan yok ise devam ediyor.
                            {
                                dispatcherCrwl.Stop();
                                saveCrawlingDic();
                                dbControl();
                                MessageBox.Show("Crawling Completed.");
                                return;
                            }
                        }
                    }

                    foreach (var vrNewUrls in hashNewUrls)
                    {
                        if (hashCurrentlyCrawlUrls.Contains(vrNewUrls)) //kontrol ediyoruz var ise devam ettiriyoruz.                   
                            continue;
                        srNewUrls = vrNewUrls; 
                        break;
                    }
                    if (string.IsNullOrEmpty(srNewUrls)) //bos mu diye bakıyoruz
                    {
                        return;
                    }

                    hashNewUrls.Remove(srNewUrls); //hashset'in içinden bu değişkeni siliyor

                    lock (hashCurrentlyCrawlUrls) //hashcurrently url bakıyor
                    {
                        hashCurrentlyCrawlUrls.Add(srNewUrls); //içine ekleme yapıyor.
                        var vrTask = Task.Factory.StartNew(() => { publicFunctions.crawlUrl(srNewUrls); }); //task.factory yeni crawl oluşturma görevi veriyor, task içinde çalıştırmasaydık çok zaman alırdı
                        lock (lstRunTask)
                        {
                            lstRunTask.Add(vrTask); //task listesine ekleme yapıyor
                        }
                    }

                }
            }



            



        }


        private void dbControl() //veritabanını kontrol etmek için
        {
            if (globalVariables.dbControl==true)
            {
                MessageBox.Show("Crawled links have been successfully added to database.");
            }
        }
        private void btnStopCrawling_Click(object sender, RoutedEventArgs e) //crawl'ı durdurdugum kısım
        {
            blCrawlStop = true; //bool func true oluyor sonra durduruyorum
            lock (hashCrawledUrls)
            {
                hashNewUrls = new HashSet<string>();
            }
            
            publicFunctions.updateListStatusBox("Web crawling have stopped, please wait.");
        }

        private void btnAddLink_Click(object sender, RoutedEventArgs e) //link ekleme kısmım
        {
            const string srLinks = "root_urls.txt";
            string link = txtLinks.Text;
            if (controls.urlControl(link)==true)
            {
                if (link.Substring(0,5)=="https" || link.Substring(0, 4) == "http")
                {
                    if (!lstLinks.Items.Contains(link) && lstLinks.Items.Count<5)
                    {
                        lstLinks.Items.Add(link);
                        using (StreamWriter saveLinks =new StreamWriter(srLinks))
                        {
                            foreach (var vrLinks in lstLinks.Items)
                            {
                                saveLinks.WriteLine(vrLinks);
                            }
                        }    
                    }
                    else
                    {
                        MessageBox.Show("You have already added this link.");
                        txtLinks.Focus();
                        return;
                    }
                    
                    txtLinks.Clear();
                    MessageBox.Show("Success, Text file is saved, you can start to crawling.");
                }
                else
                {
                    MessageBox.Show("Try Again");
                }
                
                return;
            }
            else
            {
                MessageBox.Show("Try Again");
                return;
            }
        }

        private void btnDeletLink_Click(object sender, RoutedEventArgs e) //link silme kısmım
        {
            int selection = lstLinks.SelectedIndex;
            if (selection!=-1)
            {
                var vrSelectedItem = lstLinks.Items[selection];
                MessageBox.Show($"You have deleted: {vrSelectedItem}");                
                lstLinks.Items.RemoveAt(selection);
            }
            else
            {
                MessageBox.Show("Please a select link.");
                lstLinks.Focus();
            }
        }

        private void btnSetTask_Click(object sender, RoutedEventArgs e) //task ayarladığım yer.
        {
            string srNum = txtNumberOfTasks.Text;
            if (controls.onlyNumber(srNum) == true)
            {
                if (Convert.ToInt32(srNum) <= 20)
                {
                    irMaxTaskCount=Convert.ToInt32(srNum);
                    MessageBox.Show($"You have set the task count is: {irMaxTaskCount}");
                }
                else
                {
                    MessageBox.Show("Please add number which is lower than 20. Therefore, max task count is 20.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please change your number.");
                return;
            }
        }
    }
}
