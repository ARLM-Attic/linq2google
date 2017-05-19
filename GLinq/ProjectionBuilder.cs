using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace GLinq
{
    internal class ProjectionBuilder : RestExpressionVisitor
    {
        ParameterExpression row;
        private static MethodInfo miGetValue;

        internal ProjectionBuilder()
        {
            if (miGetValue == null)
                miGetValue = typeof(ProjectionRow).GetMethod("GetValue");
        }

        internal LambdaExpression Build(Expression expression)
        {
            this.row = Expression.Parameter(typeof(ProjectionRow), "row");
            Expression body = this.Visit(expression);
            return Expression.Lambda(body, this.row);
        }

        protected override Expression VisitQueryParam(QueryParamExpression queryParam)
        {
            Expression getValue = Expression.Call(
                    this.row,
                    miGetValue,
                    Expression.Constant(queryParam.Descriptor));

            foreach (System.ComponentModel.TypeConverterAttribute converterAttr in queryParam.Converters)
            {
                Type converterType = Type.GetType(converterAttr.ConverterTypeName);
                getValue = Expression.Condition(
                    Expression.Call(Expression.New(converterType), converterType.GetMethod("CanConvertFrom", new Type[] { typeof(Type) }), Expression.Constant(typeof(string))),
                    Expression.Call(Expression.New(converterType), converterType.GetMethod("ConvertFrom", new Type[] { typeof(object) }), getValue),
                    getValue);
            }
            return Expression.Convert(
                getValue,
                queryParam.Type);
        }
        protected override Expression VisitUrlParam(UrlParamExpression urlParam)
        {
            return Expression.Constant("");
        }
    }
}
