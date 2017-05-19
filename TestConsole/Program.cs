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
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext("ABQIAAAAxKRZqmZvVzQVA-YxoFpyshT2yXp_ZAY8_ufC3CFXhHIE1NvwkxRCRRXzn1cmOT8pZPc1geUX6QNyjg");
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
