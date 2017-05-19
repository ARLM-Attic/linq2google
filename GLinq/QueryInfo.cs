using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GLinq
{
    public class QueryInfo
    {
        private IQueryParser _parser;
        public IQueryParser Parser
        {
            get { return _parser; }
        }

        private string _baseURL;
        public string BaseURL
        {
            get { return _baseURL; }
            set { _baseURL = value; }
        }

        private WebHeaderCollection _customHeaders = new WebHeaderCollection();
        public WebHeaderCollection CustomHeaders
        {
            get { return _customHeaders; }
        }

        private string _method;
        public string Method
        {
            get { return _method; }
            set { _method = value; }
        }

        private string _contentType;
        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public QueryInfo(IQueryParser parser)
        {
            _parser = parser;
        }
    }
}
