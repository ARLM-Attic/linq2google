using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //You will need a google key.  Easy to get here - http://code.google.com/apis/base/signup.html
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext(YourGoogleKey);
            var r = from ipods in gc.products
                    where ipods.BaseQuery == "mp3 players ipod video" && ipods.Brand == "apple"
                    where ipods.Price < 600
                    orderby ipods.Price ascending
                    select ipods;

            foreach (GoogleItems.Product product in r.Take(100))
            {
                Console.WriteLine("{0} for ${1}", product.Title, product.Price.ToString("#.##"));                
            }
            Console.ReadKey(true);
        }
    }
}
