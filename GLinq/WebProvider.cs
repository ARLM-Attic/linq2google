using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Net;
using System.Xml.Linq;
using System.Reflection;

namespace GLinq
{
    public class WebProvider : IProvider
    {
        #region IProvider Members

        public object Execute(Expression expression, QueryInfo info)
        {
            XElement feed = null;
            TranslateResult result = this.Translate(expression);
            Delegate projector = result.Projector.Compile();

            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(result.CommandText);
                request.Method = info.Method;
                request.ContentType = info.ContentType;
                request.Headers.Add(info.CustomHeaders);
                response = (HttpWebResponse)request.GetResponse();
                
                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(response.GetResponseStream());
                feed = XDocument.Load(reader).Element("{http://www.w3.org/2005/Atom}feed");           
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            if (feed == null)
                throw new Exception("Feed not found");
            Type elementType = TypeSystem.GetElementType(expression.Type);
            return Activator.CreateInstance(typeof(WebProjectionReader<>).MakeGenericType(elementType),
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
            LambdaExpression projector = new ProjectionBuilder().Build(proj.Projector);
            return new TranslateResult { CommandText = commandText, Projector = projector };            
        }
        
        internal class TranslateResult
        {
            internal string CommandText;
            internal LambdaExpression Projector;
        }
    }
}
