using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //You need a google key.  It's easy - http://code.google.com/apis/base/signup.html
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext(YourGoogleKey);
            var r = from ipods in gc.products
                    where ipods.BaseQuery == "mp3 players" && ipods.Brand == "apple"
                    where ipods.Price > 200 && ipods.Price < 400
                    orderby ipods.Price descending
                    select ipods;

            foreach (GoogleItems.Product product in r.Take(100))
            {
                Console.WriteLine("{0} for ${1}",product.Title, product.Price.ToString("#.##"));
            }
            Console.ReadKey(true);
        }
    }
}
