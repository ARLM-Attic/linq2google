using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using System.Xml.Linq;

namespace GLinq
{
    public class FeedAttribute : Attribute
    {
        private Type _queryParser;
        public Type QueryParser
        {
            get { return _queryParser; }
            set { _queryParser = value; }
        }

        private string _baseURL;
        public string BaseURL
        {
            get { return _baseURL; }
            set { _baseURL = value; }
        }
    }    
}
