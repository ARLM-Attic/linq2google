﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems
{
    [Feed(BaseURL = "http://gdata.youtube.com/feeds/api/videos")]
    public class YouTubeVideo : AtomEntry
    {
        private string _userName;
        [Url(DefaultValue = "scobrown")]
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        private string _vq;
        [BaseQuery(ParameterName="vq")]
        public string VideoQuery
        {
            get { return _vq; }
            set { _vq = value; }
        }
    }
}