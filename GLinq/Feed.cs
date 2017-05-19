using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

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
            _info = new QueryInfo();
            object[] attr = typeof(T).GetCustomAttributes(typeof(FeedAttribute), true);
            if (attr.Length == 1)
            {
                FeedAttribute tempAttr = attr[0] as FeedAttribute;
                if (String.IsNullOrEmpty(tempAttr.BaseURL))
                    throw new Exception("A FeedAttribute must specify the base url");

                if (tempAttr.QueryParser != null)
                    _info.Parser = (Mapping.IQueryParser)Activator.CreateInstance(tempAttr.QueryParser, tempAttr.BaseURL);
                else
                    _info.Parser = new Mapping.FeedParser(tempAttr.BaseURL);
            }
            else
                throw new Exception("A Feed query requires the object " + typeof(T).Name + " to have an attribute of type " + typeof(FeedAttribute).Name);
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
