using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Web_Crawler
{
    public class cs_fetch_result
    {
        public string srFetchSource { get; set; }
        public HttpStatusCode fetchStatusCode { get; set; }

        public string fetchStatusDesc { get; set; }

        public static string srTitle{get; set;}

        public Exception exceptionE = null;
    }
    class page_fetcher
    {
        public static cs_fetch_result fetch_a_page(string urlAddress) //sayfayı al
        {
            cs_fetch_result temp_cs_fetch_result = new cs_fetch_result();
            
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress); //istek yarat
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) //yanıt
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        StreamReader readStream = null;

                        if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                        temp_cs_fetch_result.srFetchSource = readStream.ReadToEnd();

                        temp_cs_fetch_result.fetchStatusCode = response.StatusCode;

                        //title çektigim yer
                        string regex = @"(?<=<title.*>)([\s\S]*)(?=</title>)"; //regex koşulu belirledik
                        Regex ex = new Regex(regex, RegexOptions.IgnoreCase); // regex tanılmadık 
                        cs_fetch_result.srTitle = ex.Match(temp_cs_fetch_result.srFetchSource.Trim()).ToString(); //match ettik
                    }
                }
            }
            catch (WebException e)
            {
                temp_cs_fetch_result.exceptionE = e;
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    temp_cs_fetch_result.fetchStatusCode= ((HttpWebResponse)e.Response).StatusCode;
                    temp_cs_fetch_result.fetchStatusDesc = ((HttpWebResponse)e.Response).StatusDescription;
                }
            }
            catch (Exception e)
            {          
                temp_cs_fetch_result.exceptionE = e;
            }
            return temp_cs_fetch_result;
        }
    }
}
