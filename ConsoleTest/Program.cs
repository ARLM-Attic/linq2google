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
            p.ProductsTest();
            Console.ReadKey(true);
        }
        private void ProductsTest()
        {
            //You need a google key.  It's easy - http://code.google.com/apis/base/signup.html
            Console.WriteLine("Enter your Google Base API key.  If you do not have one, its easy - http://code.google.com/apis/base/signup.html");
            Console.Write("key:");
            string key = Console.ReadLine();

            //Execute a search using the WebRequest classes
            Console.WriteLine("Execute a search using the WebRequest classes");
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext(key);
            var r = from ipods in gc.products
                    where ipods.BaseQuery == "mp3 players" && ipods.Brand == "apple" && ipods.FeedType == "snippets" && ipods.Price < 200
                    orderby ipods.Price
                    select new { ipods.Title, ipods.Price };

            foreach (var product in r.Skip(10).Take(100))
            {
                Console.WriteLine("{0} for ${1}", product.Title, product.Price.ToString("#.##"));
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            
            //Execute a search using the Syndication (Feed) classes
            Console.WriteLine("Execute a search using the Syndication (Feed) classes");
            GoogleItems.Syndication.GoogleContext sgc = new GoogleItems.Syndication.GoogleContext(key);
            var s = from ipods in sgc.Products
                    where ipods.BaseQuery == "mp3 players" && ipods.FeedType == "snippets"
                    select new { ipods.Title, ipods.Categories };

            foreach (var product in s)
            {
                Console.WriteLine("{0} has {1} category", product.Title.Text, product.Categories.Count);
                foreach (var category in product.Categories)
                    Console.WriteLine("    " + category.Name);
            }
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

            foreach (GoogleItems.Product product in r.Skip(10).Take(100))
            {
                Console.WriteLine("{0} for ${1}", product.Title, product.Price.ToString("#.##"));
            }
        }
        private void YouTubeTest()
        {
            GoogleItems.Syndication.YouTubeContext gc = new GoogleItems.Syndication.YouTubeContext();
            var r = from videos in gc.videos
                    where videos.VideoQuery == "dog" && videos.VideoQuery == "cat"
                    select videos;

            foreach (GoogleItems.Syndication.YouTubeVideo video in r.Take(2))
            {
                Console.WriteLine("{0}", video.Title);
            }
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
