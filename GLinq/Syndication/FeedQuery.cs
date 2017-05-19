using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal class FeedQuery<T> : WebQuery<T>
    {

        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private DateTime _updated;
        public DateTime Updated
        {
            get { return _updated; }
            set { _updated = value; }
        }

        private int _totalResults;
        public int TotalResults
        {
            get { return _totalResults; }
            set { _totalResults = value; }
        }

        public FeedQuery(WebContext context, QueryInfo info, Expression expression)
            : base(context, info, expression)
        {
        }
    }
}
