using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems
{
    public class GoogleContext : RestContext
    {
        public Feed<Product> products;

        public GoogleContext(string googleKey)
        {
            products = new Feed<Product>(this);
            Init(googleKey, "", "application/atom+xml", "GET");
        }

        private void Init(string googleKey, string auth, string contentType, string method)
        {
            products.Info.CustomHeaders["Authorization"] = "AuthSub token=\"" + auth + "\"";
            products.Info.CustomHeaders["X-Google-Key"] = "key=" + googleKey;
            products.Info.ContentType = contentType;
            products.Info.Method = method;
        }
    }
}
