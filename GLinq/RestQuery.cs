using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    internal class RestQuery<T> : IOrderedQueryable<T>, IQueryable<T>, IQueryProvider
    {
        private RestContext _context;
        private Expression _expression;
        private QueryInfo _info;

        public RestQuery(RestContext context, QueryInfo info, Expression expression)
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
            throw new NotImplementedException();
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
            return new RestQuery<TElement>(_context, _info, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)this._context.Provider.Execute<T>(expression, _info);            
        }

        public object Execute(Expression expression)
        {
            return this._context.Provider.Execute<T>(expression, _info);
        }

        #endregion
    }
}
