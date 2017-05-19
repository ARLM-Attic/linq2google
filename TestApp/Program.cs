using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TestApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GoogleItems.GoogleContext gc = new GoogleItems.GoogleContext("ABQIAAAAxKRZqmZvVzQVA-YxoFpyshT2yXp_ZAY8_ufC3CFXhHIE1NvwkxRCRRXzn1cmOT8pZPc1geUX6QNyjg");
            var r = from cameras in gc.products
                    where cameras.BaseQuery == "mp3 players" && cameras.Brand == "apple"
                    where cameras.Price < 600
                    orderby cameras.Price descending
                    select cameras;

            foreach (GoogleItems.Product product in r.Skip(13).Take(10))
            {
                Console.WriteLine("{0} for ${1}",product.Title, product.Price.ToString("#.##"));
            }
        }
    }
}
