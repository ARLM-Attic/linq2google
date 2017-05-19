using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq.Syndication
{
    public class Feed<T> : WebRequest<T> where T : System.ServiceModel.Syndication.SyndicationItem
    {
        public Feed(WebContext context)
            : base(context)
        {
        }


        #region IQueryProvider Members

        public override IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new FeedQuery<TElement>(this.Context, this.Info, expression);
        }

        public override IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return (IQueryable)Activator.CreateInstance(typeof(FeedQuery<>).MakeGenericType(typeof(T)), this.Context, this.Info, expression);
        }

        #endregion

    }

}
