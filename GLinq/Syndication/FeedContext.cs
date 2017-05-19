using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq.Syndication
{
    public class FeedContext : WebContext
    {
        public FeedContext()
        {
            Provider = new FeedProvider();
        }
    }
}
