using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems.Syndication
{
    public class YouTubeContext : WebContext
    {
        public WebRequest<YouTubeVideo> videos;

        public YouTubeContext()
        {
            videos = new WebRequest<YouTubeVideo>(this);
            videos.Info.ContentType = "application/atom+xml";
            videos.Info.Method = "GET";
        }
    }
}
