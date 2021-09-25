using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Web_Crawler
{
    public static class extensions
    {
        public static string normalizeUrls(this string srUrl)
        {
            return srUrl.Split('#').FirstOrDefault().ToLower(new System.Globalization.CultureInfo("en-us"));
        }
        public static string hashUrlFunc(this string srUrl)
        {
            return srUrl.normalizeUrls().ComputeSha256Hash();
        }

        public static string ComputeSha256Hash(this string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string getRootUrl(this string srUrl)
        {
            Uri myUri = new Uri(srUrl); //srUrl'yi Uri classından tanımladık
            return myUri.Host; //siteyi döndürcek örnegin hostnametype olsaydı dns döndürürdü.
        }
    }
}
