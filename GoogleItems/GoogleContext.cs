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
            Init(googleKey, "", "application/atom+xml", "GET");
        }
        public GoogleContext(string googleKey, string authToken)
        {
            Init(googleKey, authToken, "application/atom+xml", "GET");
        }

        public void Init(string googleKey, string authToken, string contentType, string method)
        {
            products = new Feed<Product>(this);
            products.Info.CustomHeaders["Authorization"] = "AuthSub token=\"" + authToken + "\"";
            products.Info.CustomHeaders["X-Google-Key"] = "key=" + googleKey;
            products.Info.ContentType = contentType;
            products.Info.Method = method;
        }
    }
}
