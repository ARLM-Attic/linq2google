using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal class QueryFormatter : RestExpressionVisitor
    {
        private NameValueCollection _urlReplacers = new NameValueCollection();
        private Dictionary<string, StringBuilder> _queryString = new Dictionary<string, StringBuilder>();
        private StringBuilder sb;
        private StringBuilder _currentQueryParam;
        private Uri _baseUri;
        private UriTemplate _uriTemplate;

        internal string Format(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            Uri uri = _uriTemplate.BindByName(_baseUri, _urlReplacers);
            System.UriBuilder builder = new UriBuilder(uri);
            if (_queryString.Count > 0)
            {
                StringBuilder queryString = new StringBuilder();
                foreach (KeyValuePair<string, StringBuilder> keyValue in _queryString)
                {
                    if (keyValue.Value != null && keyValue.Value.Length > 0)
                        queryString.AppendFormat("{0}{1}={2}", (queryString.Length > 0 ? "&" : ""), keyValue.Key, keyValue.Value.ToString());
                }
                builder.Query = queryString.ToString();
            }
            return builder.Uri.AbsoluteUri;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }
        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" -");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }
        protected override Expression VisitBinary(BinaryExpression b)
        {
            BinaryQueryStringExpression left = b.Left as BinaryQueryStringExpression;
            BinaryQueryStringExpression right = b.Right as BinaryQueryStringExpression;

            this.Visit(b.Left);
            if (left != null && right != null && _currentQueryParam != null)
            {
                switch (b.NodeType)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        _currentQueryParam.Append(" ");
                        break;
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        _currentQueryParam.Append(" | ");
                        break;
                    default:
                        throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
                }
            }
            this.Visit(b.Right);
            return b;
        }
        protected override Expression VisitBinaryQueryString(BinaryQueryStringExpression b)
        {
            QueryParamExpression qp = b.Left as QueryParamExpression;
            ConstantExpression value = null;
            if (qp == null)
            {
                qp = b.Right as QueryParamExpression;
                value = b.Left as ConstantExpression;
            }
            else
                value = b.Right as ConstantExpression;

            if (!_queryString.ContainsKey(qp.Name))
                _queryString.Add(qp.Name, new StringBuilder());

            _currentQueryParam = _queryString[qp.Name];
            _currentQueryParam.Append(qp.Descriptor.FormatQueryStringItem(value.Value.ToString(), b.Operator));

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (c.Value != null)
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(c.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                    default:
                        sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitRequest(RequestExpression request)
        {
            if (request.UrlParams != null)
            {
                foreach (UrlParamDeclaration param in request.UrlParams)
                {
                    if(!String.IsNullOrEmpty(param.Default))
                        _urlReplacers[param.Key] = param.Default;
                }
            }

            if (request.BaseUrl != null)
                Visit(request.BaseUrl);
            if (request.QueryString != null)
                Visit(request.QueryString);

            return request;
        }
        protected override Expression VisitOrderBy(OrderByExpression orderby)
        {
            if (orderby.Source != null)
                Visit(orderby.Source);

            Dictionary<string, StringBuilder> orderbyParams = orderby.FeedDescriptor.FormatSortBy(orderby.Member.Name, orderby.Direction);

            _queryString = orderbyParams.Union(_queryString.Where(x => !orderbyParams.ContainsKey(x.Key))).
                ToDictionary(x => x.Key, y => y.Value);

            return orderby;
        }
        protected override Expression VisitUri(UriExpression uri)
        {
            _baseUri = new Uri(uri.BaseUri);
            _uriTemplate = new UriTemplate(uri.UriTemplate);
            return uri;
        }
        protected override Expression VisitQueryParam(QueryParamExpression queryParam)
        {
            sb.Append(queryParam.Name);
            sb.Append("=");
            return queryParam;
        }
        protected override Expression VisitUrlParam(UrlParamExpression urlParam)
        {
            string value = urlParam.Default;
            _urlReplacers[urlParam.Key] = value;
            return urlParam;
        }
    }
}
