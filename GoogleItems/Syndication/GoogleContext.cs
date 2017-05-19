using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq.Syndication;

namespace GoogleItems.Syndication
{
    public class GoogleContext : FeedContext
    {
        public Feed<Product> Products;

        public GoogleContext(string googleKey)
        {
            Init(googleKey, "", "application/atom+xml", "GET");
        }
        public GoogleContext(string googleKey, string auth)
        {
            Init(googleKey, auth, "application/atom+xml", "GET");
        }
        private void Init(string googleKey, string auth, string contentType, string method)
        {
            Products = new Feed<Product>(this);
            Products.Info.CustomHeaders["Authorization"] = "GoogleLogin auth=\"" + auth + "\"";
            Products.Info.CustomHeaders["X-Google-Key"] = "key=" + googleKey;
            Products.Info.ContentType = contentType;
            Products.Info.Method = method;
        }
    }
}
