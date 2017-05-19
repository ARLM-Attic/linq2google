using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    internal class FeedInfo<T> : QueryInfo
    {
        private Feed<T> _feed;
        public Feed<T> Feed
        {
            get { return _feed; }
            set { _feed = value; }
        }

        public FeedInfo(IQueryParser parser)
            : base(parser)
        {
        }
    }
}
