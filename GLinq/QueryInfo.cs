using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace GLinq
{
    public class QueryInfo
    {
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

        public QueryInfo()
        {
        }
    }
}
