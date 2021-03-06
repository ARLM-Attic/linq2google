﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;
using System.ServiceModel.Syndication;

namespace GoogleItems.Syndication
{
    [Feed(BaseUri = "http://gdata.youtube.com", UriTemplate = "feeds/api/videos")]
    public class YouTubeVideo : GLinq.Syndication.FeedItem
    {
        private string _userName;
        [Url(DefaultValue = "scobrown")]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _vq;
        [QueryStringParam(ParameterName="vq")]
        public string VideoQuery
        {
            get { return _vq; }
            set { _vq = value; }
        }
    }

}
