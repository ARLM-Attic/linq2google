using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal class WebQuery<T> : IOrderedQueryable<T>, IQueryable<T>, IQueryProvider
    {
        private WebContext _context;
        private Expression _expression;
        private QueryInfo _info;

        public WebQuery(WebContext context, QueryInfo info, Expression expression)
        {
            _context = context;
            _expression = expression;
            _info = info;
        }

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return this._expression; }
        }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            return this.Execute<IEnumerable<T>>(this._expression).GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.Execute<IEnumerable<T>>(this._expression).GetEnumerator();
        }

        #endregion

        #region IQueryProvider Members

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return (IQueryable<TElement>)Activator.CreateInstance(this.GetType().GetGenericTypeDefinition().MakeGenericType(typeof(TElement)), _context, _info, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)Activator.CreateInstance(typeof(WebQuery<>).MakeGenericType(typeof(T)), _context, _info, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this._context.Provider.Execute(expression, _info);            
        }

        public object Execute(Expression expression)
        {
            return this._context.Provider.Execute(expression, _info);
        }

        #endregion

        public override string ToString()
        {
            return _context.Provider.GetQueryText(this.Expression);
        }
    }
}
