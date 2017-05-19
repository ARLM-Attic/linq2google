using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.Reflection;

namespace GLinq
{
    public class RestProvider : IProvider
    {
        #region IProvider Members

        public IEnumerable<T> Execute<T>(Expression expression, QueryInfo info)
        {
            string url = info.Parser.GetQuery(expression);
            return Execute<T>(url, info);
        }

        private IEnumerable<T> Execute<T>(string url, QueryInfo info)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = info.Method;
                request.ContentType = info.ContentType;
                request.Headers.Add(info.CustomHeaders);
                response = (HttpWebResponse)request.GetResponse();

                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(response.GetResponseStream());
                return AtomFeedSerializer.Deserialize<T>(reader, info);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

        #endregion
    }
}
