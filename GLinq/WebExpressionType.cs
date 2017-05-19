using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace GLinq
{
    internal enum WebExpressionType
    {
        Request = 1000,
        OrderBy,
        Uri,
        QueryString,
        QueryParam,
        UrlParam,
        Projection,
        BinaryQueryString
    }
    internal class OrderByExpression : Expression
    {
        private Expression _source;
        private SortDirection _direction;
        private Mapping.PropertyMapping _desc;

        internal Expression Source
        {
            get { return _source; }
        }
        internal SortDirection Direction
        {
            get { return _direction; }
        }
        internal Mapping.PropertyMapping Descriptor
        {
            get { return _desc; }
        }

        internal OrderByExpression(Type type, Expression source, SortDirection direction, Mapping.PropertyMapping desc)
            : base((ExpressionType)WebExpressionType.OrderBy, type)
        {
            _source = source;
            _direction = direction;
            _desc = desc;
        }
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
    internal class BinaryQueryStringExpression : Expression
    {
        private Expression _left;
        private Expression _right;
        private MethodInfo _method;
        private ExpressionType _oper;

        internal Expression Left
        {
            get { return _left; }
        }
        internal Expression Right
        {
            get { return _right; }
        }
        internal MethodInfo Method
        {
            get { return _method; }
        }
        internal ExpressionType Operator
        {
            get { return _oper; }
        }

        internal BinaryQueryStringExpression(Type type, ExpressionType oper, Expression left, Expression right, MethodInfo method)
            : base((ExpressionType)WebExpressionType.BinaryQueryString, type)
        {
            _left = left;
            _right = right;
            _method = method;
            _oper = oper;
        }
    }
    internal class UriExpression : Expression
    {
        private string _baseUri;
        internal string BaseUri
        {
            get { return _baseUri; }
        }

        private string _uriTemplate;
        internal string UriTemplate
        {
            get { return _uriTemplate; }
        }

        internal UriExpression(Type type, string baseUri, string uriTemplate)
            : base((ExpressionType)WebExpressionType.Uri, type)
        {
            _baseUri = baseUri;
            _uriTemplate = uriTemplate;
        }
    }
    internal class QueryStringExpression : Expression
    {
        internal QueryStringExpression(Type type)
            : base((ExpressionType)WebExpressionType.QueryString, type)
        {
        }
    }
    internal class QueryParamExpression : Expression
    {
        private string _name;
        private Mapping.PropertyMapping _desc;

        internal Mapping.PropertyMapping Descriptor
        {
            get { return _desc; }
        }

        internal QueryParamExpression(Type type, Mapping.PropertyMapping desc)
            : base((ExpressionType)WebExpressionType.QueryParam, type)
        {
            _desc = desc;
        }
    }
    internal class QueryParamDeclaration
    {
        private string _name;
        private Expression _expression;

        internal string Name
        {
            get { return _name; }
        }
        internal Expression Expression
        {
            get { return _expression; }
        }

        internal QueryParamDeclaration(string name, Expression expression)
        {
            _name = name;
            _expression = expression;
        }
    }

    internal class UrlParamExpression : Expression
    {
        private string _default;
        private Mapping.PropertyMapping _desc;

        internal Mapping.PropertyMapping Descriptor
        {
            get { return _desc; }
        }
        internal string Key
        {
            get { return _desc.UrlVariableName; }
        }
        internal string Default
        {
            get { return _default; }
        }

        internal UrlParamExpression(Type type, Mapping.PropertyMapping desc, string defaultValue)
            : base((ExpressionType)WebExpressionType.UrlParam, type)
        {
            _default = defaultValue;
            _desc = desc;
        }
    }
    internal class UrlParamDeclaration
    {
        private string _key;
        private string _default;
        private Expression _expression;

        internal string Key
        {
            get { return _key; }
        }
        internal string Default
        {
            get { return _default; }
        }
        internal Expression Expression
        {
            get { return _expression; }
        }

        internal UrlParamDeclaration(string key, string defaultValue, Expression expression)
        {
            _key = key;
            _default = defaultValue;
            _expression = expression;
        }
    }

    internal class RequestExpression : Expression
    {
        private Expression _baseUrl;
        private Expression _queryString;
        private ReadOnlyCollection<QueryParamDeclaration> _queryParams;
        private ReadOnlyCollection<UrlParamDeclaration> _urlParams;
        private Expression _take;
        private Expression _skip;

        internal Expression Take
        {
            get { return _take; }
            set { _take = value; }
        }
        internal Expression Skip
        {
            get { return _skip; }
            set { _skip = value; }
        }
        internal Expression BaseUrl
        {
            get { return _baseUrl; }
        }
        internal Expression QueryString
        {
            get { return _queryString; }
        }
        internal ReadOnlyCollection<QueryParamDeclaration> QueryParams
        {
            get { return _queryParams; }
        }
        internal ReadOnlyCollection<UrlParamDeclaration> UrlParams
        {
            get { return _urlParams; }
        }

        internal RequestExpression(Type type, IEnumerable<QueryParamDeclaration> queryParams, IEnumerable<UrlParamDeclaration> urlParams, Expression baseUrl, Expression queryString)
            : base((ExpressionType)WebExpressionType.Request, type)
        {
            _queryParams = queryParams as ReadOnlyCollection<QueryParamDeclaration>;
            _urlParams = urlParams as ReadOnlyCollection<UrlParamDeclaration>;
            _baseUrl = baseUrl;
            _queryString = queryString;

            if (_queryParams == null)
                _queryParams = new List<QueryParamDeclaration>(queryParams).AsReadOnly();
        }
    }
    internal class ProjectionExpression : Expression
    {
        Expression _source;
        Expression _projector;

        internal Expression Source
        {
            get { return this._source; }
        }
        internal Expression Projector
        {
            get { return this._projector; }
        }

        internal ProjectionExpression(Expression source, Expression projector)
            : base((ExpressionType)WebExpressionType.Projection, projector.Type)
        {
            _source = source;
            _projector = projector;
        }
    }
}
