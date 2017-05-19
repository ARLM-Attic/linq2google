using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal sealed class ProjectedQueryParams
    {
        private Expression _projector;
        private ReadOnlyCollection<QueryParamDeclaration> _queryParams;

        internal Expression Projector
        {
            get { return _projector; }
        }
        internal ReadOnlyCollection<QueryParamDeclaration> Params
        {
            get { return _queryParams; }
        }

        internal ProjectedQueryParams(Expression projector, ReadOnlyCollection<QueryParamDeclaration> queryParams)
        {
            _projector = projector;
            _queryParams = queryParams;
        }
    }

    internal class QueryParamProjector : RestExpressionVisitor
    {
        private Nominator _nominator;
        private Dictionary<QueryParamExpression, QueryParamExpression> _map;
        private List<QueryParamDeclaration> _queryParams;
        private HashSet<string> _paramNames;
        private HashSet<Expression> _candidates;

        internal QueryParamProjector(Func<Expression, bool> fnCanBeColumn)
        {
            _nominator = new Nominator(fnCanBeColumn);
        }

        internal ProjectedQueryParams ProjectQueryParams(Expression expression)
        {
            _map = new Dictionary<QueryParamExpression, QueryParamExpression>();
            _queryParams = new List<QueryParamDeclaration>();
            _paramNames = new HashSet<string>();
            _candidates = _nominator.Nominate(expression);
            return new ProjectedQueryParams(Visit(expression), _queryParams.AsReadOnly());
        }

        protected override Expression Visit(Expression expression)
        {
            if (this._candidates.Contains(expression))
            {
                if (expression.NodeType == (ExpressionType)WebExpressionType.QueryParam)
                {
                    QueryParamExpression param = (QueryParamExpression)expression;
                    QueryParamExpression mapped;
                    if (_map.TryGetValue(param, out mapped))
                        return mapped;

                    int ordinal = _queryParams.Count;
                    _queryParams.Add(new QueryParamDeclaration(param.Descriptor.ParameterName, param));
                    mapped = new QueryParamExpression(param.Type, param.Descriptor);
                    _map[param] = mapped;
                    _paramNames.Add(param.Descriptor.ParameterName);
                    return mapped;
                }
                else
                {
                    throw new Exception("not sure how I got here");
                }
            }
            else
            {
                return base.Visit(expression);
            }
        }


        class Nominator : RestExpressionVisitor
        {
            private Func<Expression, bool> _fnCanBeQueryParam;
            private bool _isBlocked;
            private HashSet<Expression> _candidates;

            internal Nominator(Func<Expression, bool> fnCanBeQueryParam)
            {
                _fnCanBeQueryParam = fnCanBeQueryParam;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                _candidates = new HashSet<Expression>();
                _isBlocked = false;
                Visit(expression);
                return _candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveIsBlocked = _isBlocked;
                    _isBlocked = false;
                    base.Visit(expression);
                    if (!_isBlocked)
                    {
                        if (_fnCanBeQueryParam(expression))
                        {
                            _candidates.Add(expression);
                        }
                        else
                        {
                            _isBlocked = true;
                        }
                    }
                    _isBlocked |= saveIsBlocked;
                }
                return expression;
            }
        }
    }
}
