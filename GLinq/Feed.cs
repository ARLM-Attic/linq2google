using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    public class Feed<T> : IOrderedQueryable<T>, IQueryable<T>, IQueryProvider
    {
        private RestContext _context;
        private QueryInfo _info;
        public QueryInfo Info
        {
            get { return _info; }
        }

        public Feed(RestContext context)
        {
            _context = context;
            object[] attrs = typeof(T).GetCustomAttributes(typeof(FeedAttribute), true);
            if(attrs.Length == 1)
            {
                FeedAttribute feedAttr = attrs[0] as FeedAttribute;
                if(!string.IsNullOrEmpty(feedAttr.BaseURL))
                {
                    _info = new QueryInfo((IQueryParser)Activator.CreateInstance(feedAttr.QueryParser, feedAttr.BaseURL));
                }
                else
                    throw new Exception("The BaseURL must be specified for attribute " + typeof(FeedAttribute).Name);
            }
            else
                throw new Exception("The type " + typeof(T).Name + " does not have an attribute of type " + typeof(FeedAttribute).Name);
            
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return null;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public Expression Expression
        {
            get { return Expression.Constant(this); }
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

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new RestQuery<TElement>(_context, _info, expression);
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return (TResult)this._context.Provider.Execute<T>(expression, _info);
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return this._context.Provider.Execute<T>(expression, _info);
        }

        #endregion

    }
}
