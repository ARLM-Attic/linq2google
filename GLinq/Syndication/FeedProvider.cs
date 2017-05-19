using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.ServiceModel.Syndication;

namespace GLinq.Syndication
{
    public class FeedProvider : IProvider
    {
        #region IProvider Members

        public object Execute(Expression expression, QueryInfo info)
        {
            TranslateResult result = this.Translate(expression);
            Delegate projector = result.Projector.Compile();
            XElement feed = null;
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(result.CommandText);
                request.Method = info.Method;
                request.ContentType = info.ContentType;
                request.Headers.Add(info.CustomHeaders);
                response = (HttpWebResponse)request.GetResponse();

                //Type elementType = TypeSystem.GetElementType(expression.Type);
                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(response.GetResponseStream());
                feed = XDocument.Load(reader).Element("{http://www.w3.org/2005/Atom}feed");        
                //return SyndicationFeed.Load<FeedType>(reader).Items;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            Type elementType = TypeSystem.GetElementType(expression.Type);
            return Activator.CreateInstance(typeof(FeedProjectionReader<>).MakeGenericType(elementType),
                feed, projector);
        }

        public string GetQueryText(Expression expression)
        {
            return Translate(expression).CommandText;
        }

        #endregion

        private TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            ProjectionExpression proj = (ProjectionExpression)new QueryBinder().Bind(expression);
            string commandText = new QueryFormatter().Format(proj.Source);
            LambdaExpression projector = new FeedProjectionBuilder().Build(proj.Projector);
            return new TranslateResult { CommandText = commandText, Projector = projector };
        }

        internal class TranslateResult
        {
            internal string CommandText;
            internal LambdaExpression Projector;
        }
    }

    public class FeedType : SyndicationFeed
    {
        protected override SyndicationItem CreateItem()
        {
            return base.CreateItem();
        }
    }
}
