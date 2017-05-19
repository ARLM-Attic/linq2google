using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal sealed class ProjectedUrlParams
    {
        private Expression _projector;
        private ReadOnlyCollection<UrlParamDeclaration> _urlParams;

        internal Expression Projector
        {
            get { return _projector; }
        }
        internal ReadOnlyCollection<UrlParamDeclaration> Params
        {
            get { return _urlParams; }
        }

        internal ProjectedUrlParams(Expression projector, ReadOnlyCollection<UrlParamDeclaration> urlParams)
        {
            _projector = projector;
            _urlParams = urlParams;
        }
    }

    internal class UrlParamProjector : RestExpressionVisitor
    {
        private Nominator _nominator;
        private Dictionary<UrlParamExpression, UrlParamExpression> _map;
        private List<UrlParamDeclaration> _urlParams;
        private HashSet<string> _paramNames;
        private HashSet<Expression> _candidates;

        internal UrlParamProjector(Func<Expression, bool> fnCanBeColumn)
        {
            _nominator = new Nominator(fnCanBeColumn);
        }

        internal ProjectedUrlParams ProjectUrlParams(Expression expression)
        {
            _map = new Dictionary<UrlParamExpression, UrlParamExpression>();
            _urlParams = new List<UrlParamDeclaration>();
            _paramNames = new HashSet<string>();
            _candidates = _nominator.Nominate(expression);
            return new ProjectedUrlParams(Visit(expression), _urlParams.AsReadOnly());
        }

        protected override Expression Visit(Expression expression)
        {
            if (this._candidates.Contains(expression))
            {
                if (expression.NodeType == (ExpressionType)WebExpressionType.UrlParam)
                {
                    UrlParamExpression param = (UrlParamExpression)expression;
                    UrlParamExpression mapped;
                    if (_map.TryGetValue(param, out mapped))
                        return mapped;

                    int ordinal = _urlParams.Count;
                    _urlParams.Add(new UrlParamDeclaration(param.Key, param.Default, param));
                    mapped = new UrlParamExpression(param.Type, param.Descriptor, param.Default);
                    _map[param] = mapped;
                    _paramNames.Add(param.Key);
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
            private Func<Expression, bool> _fnCanBeUrlParam;
            private bool _isBlocked;
            private HashSet<Expression> _candidates;

            internal Nominator(Func<Expression, bool> fnCanBeUrlParam)
            {
                _fnCanBeUrlParam = fnCanBeUrlParam;
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
                        if (_fnCanBeUrlParam(expression))
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
