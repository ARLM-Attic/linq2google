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
    }
}
