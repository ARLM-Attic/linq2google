using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class FeedAttribute : Attribute
    {
        private string _baseURL;
        public string BaseURL
        {
            get { return _baseURL; }
            set { _baseURL = value; }
        }

        private Type _queryParser = typeof(FeedParser);
        public Type QueryParser
        {
            get { return _queryParser; }
            set 
            { 
                if(value == null)
                    throw new ArgumentNullException("value");
                _queryParser = value; 
            }
        }

        private string _defaultParameterName = "";
        public string DefaultParameterName
        {
            get { return _defaultParameterName; }
            set { _defaultParameterName = value; }
        }
    }
}
