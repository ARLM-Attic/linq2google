using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using GLinq.Mapping;

namespace GLinq
{
    internal class QueryBinder : RestExpressionVisitor
    {
        Dictionary<ParameterExpression, Expression> _map;
        QueryParamProjector _queryParamsProjector;
        UrlParamProjector _urlParamsProjector;

        internal QueryBinder()
        {
            _queryParamsProjector = new QueryParamProjector(this.CanBeQueryParam);
            _urlParamsProjector = new UrlParamProjector(this.CanBeUrlParam);
        }

        private bool CanBeQueryParam(Expression expression)
        {
            return expression.NodeType == (ExpressionType)WebExpressionType.QueryParam;
        }
        private bool CanBeUrlParam(Expression expression)
        {
            return expression.NodeType == (ExpressionType)WebExpressionType.UrlParam;
        }

        internal Expression Bind(Expression expression)
        {
            this._map = new Dictionary<ParameterExpression, Expression>();
            return this.Visit(expression);
        }

        private FeedAttribute GetFeedAttribute(Type resultType)
        {
            object[] attrs = resultType.GetCustomAttributes(typeof(FeedAttribute), false);
            if (attrs.Length == 0)
                throw new Exception("A FeedAttribute is required for type " + resultType.Name);

            return (FeedAttribute)attrs[0];
        }

        private string GetQueryParamName(MemberInfo member, QueryStringParamAttribute attr)
        {
            if (!String.IsNullOrEmpty(attr.ParameterName))
                return attr.ParameterName;
            object[] attrs = member.DeclaringType.GetCustomAttributes(typeof(FeedAttribute), false);
            if (attrs.Length > 0 && !String.IsNullOrEmpty(((FeedAttribute)attrs[0]).DefaultParameterName))
                return ((FeedAttribute)attrs[0]).DefaultParameterName;

            return member.Name;
        }

        private Dictionary<object, MemberInfo> GetMappedMembers(Type rowType)
        {
            Dictionary<object, MemberInfo> members = new Dictionary<object, MemberInfo>();            
            foreach (PropertyMapping info in rowType.GetMapping().Properties)
            {
                if (info.ParamAttributes.Count > 0)
                {
                    Type paramType = info.PropertyInfo.PropertyType;
                    QueryParamDeclaration param = new QueryParamDeclaration(info.ElementName, new QueryParamExpression(paramType, info));
                    members.Add(param, info.PropertyInfo);
                }
                if (info.UrlAttributes.Count > 0)
                {
                    string defaultValue = ((UrlAttribute)info.UrlAttributes[0]).DefaultValue;
                    Type paramType = info.PropertyInfo.PropertyType;
                    UrlParamDeclaration param = new UrlParamDeclaration(info.UrlVariableName, defaultValue, new UrlParamExpression(paramType, info, defaultValue));
                    members.Add(param, info.PropertyInfo);
                }
            }
            return members;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) ||
                m.Method.DeclaringType == typeof(Enumerable))
            {
                switch (m.Method.Name)
                {
                    case "Where":
                        return this.BindWhere(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                    case "Select":
                        return this.BindSelect(m.Type, m.Arguments[0], (LambdaExpression)StripQuotes(m.Arguments[1]));
                    case "OrderBy":
                        return this.OrderBy(m.Type, m.Arguments[0], m.Arguments[1], SortDirection.Ascending);
                    case "OrderByDescending":
                        return this.OrderBy(m.Type, m.Arguments[0], m.Arguments[1], SortDirection.Descending);
                    case "Take":
                        return this.Take(m.Type, m.Arguments[0]);
                    case "Skip":
                        return this.Skip(m.Type, m.Arguments[0]);
                }
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
            }
            return base.VisitMethodCall(m);
        }

        private Expression BindWhere(Type resultType, Expression source, LambdaExpression predicate)
        {
            ProjectionExpression projection = (ProjectionExpression)this.Visit(source);
            this._map[predicate.Parameters[0]] = projection.Projector;
            Expression where = this.Visit(predicate.Body);
            ProjectedQueryParams pqp = this.ProjectQueryParams(projection.Projector);
            ProjectedUrlParams pup = this.ProjectUrlParams(projection.Projector);
            return new ProjectionExpression(
                new RequestExpression(resultType, pqp.Params, pup.Params, projection.Source, where),
                pqp.Projector
                );
        }

        private Expression BindSelect(Type resultType, Expression source, LambdaExpression selector)
        {
            ProjectionExpression projection = (ProjectionExpression)this.Visit(source);
            this._map[selector.Parameters[0]] = projection.Projector;
            Expression expression = this.Visit(selector.Body);
            ProjectedQueryParams pqp = this.ProjectQueryParams(expression);
            ProjectedUrlParams pup = this.ProjectUrlParams(expression);
            return new ProjectionExpression(
                new RequestExpression(resultType, pqp.Params, pup.Params, projection.Source, null),
                pqp.Projector
                );
        }

        private Expression OrderBy(Type resultType, Expression source, Expression parameter, SortDirection direction)
        {
            LambdaExpression lambda = (LambdaExpression)((UnaryExpression)parameter).Operand;
            Type elementType = (lambda.Parameters[0].Type);
            MemberInfo member = ((MemberExpression)lambda.Body).Member;
            ProjectionExpression projection = (ProjectionExpression)this.Visit(source);
            ProjectedQueryParams pqp = this.ProjectQueryParams(projection.Projector);
            return new ProjectionExpression(new OrderByExpression(resultType, projection.Source, direction, elementType.GetMapping().GetProperty(member)), pqp.Projector);
        }

        private Expression Take(Type resultType, Expression source)
        {
            return Visit(source);
        }

        private Expression Skip(Type resultType, Expression source)
        {
            return Visit(source);
        }

        private ProjectedQueryParams ProjectQueryParams(Expression expression)
        {
            return this._queryParamsProjector.ProjectQueryParams(expression);
        }
        private ProjectedUrlParams ProjectUrlParams(Expression expression)
        {
            return this._urlParamsProjector.ProjectUrlParams(expression);
        }

        private bool IsRequest(object value)
        {
            IQueryable q = value as IQueryable;
            return q != null && q.Expression.NodeType == ExpressionType.Constant;
        }
        private ProjectionExpression GetRequestProjection(object value)
        {
            IQueryable request = (IQueryable)value;
            List<MemberBinding> bindings = new List<MemberBinding>();
            List<QueryParamDeclaration> queryParams = new List<QueryParamDeclaration>();
            List<UrlParamDeclaration> urlParams = new List<UrlParamDeclaration>();
            foreach (KeyValuePair<object, MemberInfo> mi in this.GetMappedMembers(request.ElementType))
            {
                if (mi.Key is QueryParamDeclaration)
                {
                    bindings.Add(Expression.Bind(mi.Value, ((QueryParamDeclaration)mi.Key).Expression));
                    queryParams.Add((QueryParamDeclaration)mi.Key);
                }
                else if (mi.Key is UrlParamDeclaration)
                {
                    bindings.Add(Expression.Bind(mi.Value, ((UrlParamDeclaration)mi.Key).Expression));
                    urlParams.Add((UrlParamDeclaration)mi.Key);
                }
            }
            Expression projector = Expression.MemberInit(Expression.New(request.ElementType), bindings);
            Type resultType = typeof(IEnumerable<>).MakeGenericType(request.ElementType);
            FeedAttribute feedAttr = GetFeedAttribute(request.ElementType);
            return new ProjectionExpression(
                new RequestExpression(
                    resultType,
                    queryParams,
                    urlParams,
                    new UriExpression(resultType, feedAttr.BaseUri, feedAttr.UriTemplate),
                    null
                    ),
                projector
                );
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            if (this.IsRequest(c.Value))
                return GetRequestProjection(c.Value);
            return c;
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            Expression e;
            if (this._map.TryGetValue(p, out e))
                return e;
            return p;
        }
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Expression source = this.Visit(m.Expression);
            switch (source.NodeType)
            {
                case ExpressionType.MemberInit:
                    MemberInitExpression min = (MemberInitExpression)source;
                    for (int i = 0, n = min.Bindings.Count; i < n; i++)
                    {
                        MemberAssignment assign = min.Bindings[i] as MemberAssignment;
                        if (assign != null && MembersMatch(assign.Member, m.Member))
                            return assign.Expression;
                    }
                    break;
                case ExpressionType.New:
                    NewExpression nex = (NewExpression)source;
                    if (nex.Members != null)
                    {
                        for (int i = 0, n = nex.Members.Count; i < n; i++)
                        {
                            if (MembersMatch(nex.Members[i], m.Member))
                                return nex.Arguments[i];
                        }
                    }
                    break;
            }
            if (source == m.Expression)
                return m;
            return MakeMemberAccess(source, m.Member);
        }

        private bool MembersMatch(MemberInfo a, MemberInfo b)
        {
            if (a == b)
                return true;
            if (a is MethodInfo && b is PropertyInfo)
                return a == ((PropertyInfo)b).GetGetMethod();
            else if (a is PropertyInfo && b is MethodInfo)
                return ((PropertyInfo)a).GetGetMethod() == b;
            return false;
        }
        private Expression MakeMemberAccess(Expression source, MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null)
            {
                return Expression.Field(source, fi);
            }
            PropertyInfo pi = (PropertyInfo)mi;
            return Expression.Property(source, pi);
        }
    }
}
