using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace GLinq.Syndication
{
    internal class FeedProjectionBuilder : RestExpressionVisitor
    {
        private ParameterExpression _syndicationItem;

        internal FeedProjectionBuilder()
        {
        }

        internal LambdaExpression Build(Expression expression)
        {
            _syndicationItem = Expression.Parameter(typeof(System.Xml.Linq.XElement), "row");
            Expression body = this.Visit(expression);
            return Expression.Lambda(body, _syndicationItem);
        }

        protected override Expression VisitMemberInit(MemberInitExpression init)
        {
            if(init.Type.IsSubclassOf(typeof(System.ServiceModel.Syndication.SyndicationItem)))
            {
                return Expression.Call(GetGenericMember(init.Type, "Load").MakeGenericMethod(init.Type), 
                        Expression.Call(_syndicationItem, _syndicationItem.Type.GetMethod("CreateReader"), null));                
            }
            return base.VisitMemberInit(init);
        }

        private MethodInfo GetGenericMember(Type type, string memberName)
        {
            foreach(MemberInfo mi in type.GetMember(memberName, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public))
            {
                MethodInfo method = mi as MethodInfo;
                if (method != null && method.IsGenericMethod)
                {
                    return method;
                }
            }
            return null;
        }
    }
}
