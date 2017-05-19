using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal class RestExpressionVisitor : ExpressionVisitor
    {
        protected override Expression Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch ((WebExpressionType)exp.NodeType)
            {
                case WebExpressionType.Request:
                    return this.VisitRequest((RequestExpression)exp);
                case WebExpressionType.OrderBy:
                    return this.VisitOrderBy((OrderByExpression)exp);
                case WebExpressionType.Uri:
                    return this.VisitUri((UriExpression)exp);
                case WebExpressionType.QueryString:
                    return this.VisitQueryString((QueryStringExpression)exp);
                case WebExpressionType.QueryParam:
                    return this.VisitQueryParam((QueryParamExpression)exp);
                case WebExpressionType.UrlParam:
                    return this.VisitUrlParam((UrlParamExpression)exp);
                case WebExpressionType.Projection:
                    return this.VisitProjection((ProjectionExpression)exp);
                case WebExpressionType.BinaryQueryString:
                    return this.VisitBinaryQueryString((BinaryQueryStringExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitRequest(RequestExpression request)
        {
            Expression baseUrl = this.Visit(request.BaseUrl);
            Expression queryString = this.Visit(request.QueryString);
            ReadOnlyCollection<QueryParamDeclaration> queryParams = this.VisitQueryParamDeclarations(request.QueryParams);
            ReadOnlyCollection<UrlParamDeclaration> urlParams = this.VisitUrlParamDeclarations(request.UrlParams);
            if (baseUrl != request.BaseUrl || queryString != request.QueryString || queryParams != request.QueryParams || urlParams != request.UrlParams)
                return new RequestExpression(request.Type, queryParams, urlParams, baseUrl, queryString);
            return request;
        }
        protected virtual Expression VisitOrderBy(OrderByExpression orderby)
        {
            Expression source = this.Visit(orderby.Source);
            if (source != orderby.Source)
                return new OrderByExpression(orderby.Type, source, orderby.Direction, orderby.Descriptor);
            return orderby;
        }
        protected virtual Expression VisitUri(UriExpression uri)
        {
            return uri;
        }
        protected virtual Expression VisitQueryString(QueryStringExpression queryString)
        {
            return queryString;
        }
        protected virtual Expression VisitQueryParam(QueryParamExpression queryParam)
        {
            return queryParam;
        }
        protected virtual Expression VisitUrlParam(UrlParamExpression urlParam)
        {
            return urlParam;
        }
        protected virtual Expression VisitProjection(ProjectionExpression proj)
        {
            RequestExpression source = (RequestExpression)Visit(proj.Source);
            Expression projector = this.Visit(proj.Projector);
            if (source != proj.Source || projector != proj.Projector)
                return new ProjectionExpression(source, projector);
            return proj;
        }
        protected ReadOnlyCollection<QueryParamDeclaration> VisitQueryParamDeclarations(ReadOnlyCollection<QueryParamDeclaration> queryParams)
        {
            List<QueryParamDeclaration> alternate = null;
            for (int i = 0, n = queryParams.Count; i < n; i++)
            {
                QueryParamDeclaration param = queryParams[i];
                Expression e = this.Visit(param.Expression);
                if (alternate == null && e != param.Expression)
                    alternate = queryParams.Take(i).ToList();
                if (alternate != null)
                    alternate.Add(new QueryParamDeclaration(param.Name, e));
            }
            if (alternate != null)
                return alternate.AsReadOnly();

            return queryParams;
        }
        protected ReadOnlyCollection<UrlParamDeclaration> VisitUrlParamDeclarations(ReadOnlyCollection<UrlParamDeclaration> urlParams)
        {
            List<UrlParamDeclaration> alternate = null;
            for (int i = 0, n = urlParams.Count; i < n; i++)
            {
                UrlParamDeclaration param = urlParams[i];
                Expression e = this.Visit(param.Expression);
                if (alternate == null && e != param.Expression)
                    alternate = urlParams.Take(i).ToList();
                if (alternate != null)
                    alternate.Add(new UrlParamDeclaration(param.Key, param.Default, e));
            }
            if (alternate != null)
                return alternate.AsReadOnly();

            return urlParams;
        }
        protected override Expression VisitBinary(BinaryExpression b)
        {
            b = (BinaryExpression)base.VisitBinary(b);
            if (b.Left.NodeType == (ExpressionType)WebExpressionType.QueryParam || b.Right.NodeType == (ExpressionType)WebExpressionType.QueryParam)
                return new BinaryQueryStringExpression(b.Type, b.NodeType, b.Left, b.Right, b.Method);

            return b;
        }
        protected virtual Expression VisitBinaryQueryString(BinaryQueryStringExpression b)
        {
            Expression left = this.Visit(b.Left);
            Expression right = this.Visit(b.Right);
            if (left != b.Left || right != b.Right)
            {
                return new BinaryQueryStringExpression(b.Type, b.NodeType, left, right, b.Method);
            }
            return b;
        }
    }
}
