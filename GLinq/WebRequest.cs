using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    public class WebRequest<T> : IOrderedQueryable<T>, IOrderedQueryable, IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IQueryProvider
    {
        private Expression _expression;
        
        private WebContext _context;
        protected virtual WebContext Context
        {
            get { return _context; }
        }

        private QueryInfo _info;
        public QueryInfo Info
        {
            get { return _info; }
        }

        public WebRequest(WebContext context)
        {
            _expression = Expression.Constant(this);
            _context = context;
            _info = new QueryInfo();
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.Execute<IEnumerable<T>>(this._expression).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Execute<IEnumerable<T>>(this._expression).GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get 
            {
                return this;
            }
        }

        #endregion

        #region IQueryProvider Members

        public virtual IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new WebQuery<TElement>(_context, _info, expression);
        }

        public virtual IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return (IQueryable)Activator.CreateInstance(typeof(WebQuery<>).MakeGenericType(typeof(T)), _context, _info, expression);
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return (TResult)this._context.Provider.Execute(expression, _info);
        }

        public object Execute(System.Linq.Expressions.Expression expression)
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
