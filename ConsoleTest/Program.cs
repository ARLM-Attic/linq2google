using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.LoginProductsTest();
            Console.ReadKey(true);
        }
        private void ProductsTest()
        {
            //You need a google key.  It's easy - http://code.google.com/apis/base/signup.html
            Console.WriteLine("Enter your Google Base API key.  If you do not have one, its easy - http://code.google.com/apis/base/signup.html");
            Console.Write("key:");
            string key = Console.ReadLine();
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext(key);
            var r = from ipods in gc.products
                    where ipods.BaseQuery == "mp3 players" && ipods.BaseQuery == "apple"
                    where ipods.Price > 200
                    orderby ipods.Price descending
                    select ipods;

            //The feed does not have any changes yet - the update time has not been changed
            Console.WriteLine(gc.products.Updated.ToLocalTime());
            foreach (GoogleItems.Product product in r.Skip(10).Take(100))
            {
                Console.WriteLine("{0} for ${1}", product.Title, product.Price.ToString("#.##"));
            }
            //Now the feed has changes once we have made the query
            Console.WriteLine(gc.products.Updated.ToLocalTime());
        }
        private void LoginProductsTest()
        {
            string token = DoLogin("gbase");
            //You need a google key.  It's easy - http://code.google.com/apis/base/signup.html
            Console.WriteLine("Enter your Google Base API key.  If you do not have one, its easy - http://code.google.com/apis/base/signup.html");
            Console.Write("key:");
            string key = Console.ReadLine();
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext(key, token);
            var r = from ipods in gc.products
                    where ipods.FeedType == "items"
                    select ipods;

            //The feed does not have any changes yet - the update time has not been changed
            Console.WriteLine(gc.products.Updated.ToLocalTime());
            foreach (GoogleItems.Product product in r.Skip(10).Take(100))
            {
                Console.WriteLine("{0} for ${1}", product.Title, product.Price.ToString("#.##"));
            }
            //Now the feed has changes once we have made the query
            Console.WriteLine(gc.products.Updated.ToLocalTime());
        }
        private void YouTubeTest()
        {
            GoogleItems.YouTubeContext gc = new GoogleItems.YouTubeContext();
            var r = from videos in gc.videos
                    where videos.VideoQuery == "dog" && videos.VideoQuery == "cat"
                    select videos;

            //The feed does not have any changes yet - the update time has not been changed
            Console.WriteLine(gc.videos.Updated.ToLocalTime());
            foreach (GoogleItems.YouTubeVideo video in r.Take(2))
            {
                Console.WriteLine("{0}", video.Title);
            }
            //Now the feed has changes once we have made the query
            Console.WriteLine(gc.videos.Updated.ToLocalTime());
        }

        public string DoLogin(string service)
        {
            Console.Write("Email:");
            string email = Console.ReadLine();
            Console.Write("Password:");
            string password = Console.ReadLine();

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("https://www.google.com/accounts/ClientLogin");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(request.GetRequestStream());
            sw.Write("accountType=HOSTED_OR_GOOGLE&Email=" + email + "&Passwd=" + password + "&service=" + service + "&source=GLinq-GLinqDemo-1.00");
            sw.Flush();
            sw.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if(line.StartsWith("Auth"))
                        return line.Replace("Auth=", "");
                }
            }
            finally
            {
                if(response != null)
                    response.Close();
            }
            return null;
        }
    }
}
