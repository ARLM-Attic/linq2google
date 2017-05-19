using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace GLinq.Mapping
{
    internal class FeedParser : IQueryParser
    {
        private StringBuilder _baseQuery = new StringBuilder();
        private StringBuilder _orderbyQuery = new StringBuilder();
        private StringBuilder _sortQuery = new StringBuilder();
        private int _takeQuery = -1;
        private int _skipQuery = -1;
        private string _baseURL;

        #region IQueryParser Members

        public string GetQuery(Expression expression)
        {
            ExpandExpression(expression, null);
            string query = "";
            if (_baseQuery.Length > 0)
                query += "bq=" + _baseQuery.ToString();
            if (_orderbyQuery.Length > 0)
                query += String.Format("{0}orderby={1}", (query.Length > 0 ? "&" : ""), _orderbyQuery.ToString());
            if (_orderbyQuery.Length > 0)
                query += String.Format("{0}sortorder={1}", (query.Length > 0 ? "&" : ""), _sortQuery.ToString());
            if (_takeQuery > 0)
                query += String.Format("{0}max-results={1}", (query.Length > 0 ? "&" : ""), _takeQuery.ToString());
            if (_skipQuery > 0)
                query += String.Format("{0}start-index={1}", (query.Length > 0 ? "&" : ""), _skipQuery.ToString());

            return _baseURL + "?" + query;
        }

        #endregion

        public FeedParser(string baseURL)
        {
            _baseURL = baseURL;
        }

        private void Where(MethodCallExpression method)
        {
            LambdaExpression lamda = ((UnaryExpression)method.Arguments[1]).Operand as LambdaExpression;
            ExpandExpression(lamda.Body, _baseQuery);
        }
        private void OrderBy(MethodCallExpression method)
        {
            LambdaExpression lamda = ((UnaryExpression)method.Arguments[1]).Operand as LambdaExpression;
            ExpandExpression(lamda.Body, _orderbyQuery);
        }
        private void SortOrder(string order)
        {
            _sortQuery.Append(order);
        }
        private void Take(MethodCallExpression method)
        {
            _takeQuery = (int)((ConstantExpression)method.Arguments[1]).Value;
        }
        private void Skip(MethodCallExpression method)
        {
            _skipQuery = (int)((ConstantExpression)method.Arguments[1]).Value;
        }

        private void ExpandExpression(Expression e, StringBuilder query)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    BinaryExpression be = e as BinaryExpression;
                    object[] customAttributes = ((MemberExpression)be.Left).Member.GetCustomAttributes(true);
                    if (customAttributes[0] is BaseQueryAttribute)
                    {
                        ExpandExpression(be.Right, query);
                    }
                    else if (customAttributes[0] is AttributeItemAttribute)
                    {
                        query.Append(" [");
                        BinaryExpressionType(e as BinaryExpression, ":", query);
                        query.Append("]");
                    }
                    break;
                case ExpressionType.NotEqual:
                    be = e as BinaryExpression;
                    customAttributes = ((MemberExpression)be.Left).Member.GetCustomAttributes(true);
                    if (customAttributes[0] is BaseQueryAttribute)
                    {
                        query.Append("-");
                        ExpandExpression(be.Right, query);
                    }
                    else if (customAttributes[0] is AttributeItemAttribute)
                    {
                        query.Append(" [");
                        BinaryExpressionType(e as BinaryExpression, "!=", query);
                        query.Append("]");
                    }
                    break;
                case ExpressionType.OrElse:
                    be = e as BinaryExpression;
                    query.Append("(");
                    BinaryExpressionType(e as BinaryExpression, "|", query);
                    query.Append(")");
                    break;
                case ExpressionType.AndAlso:
                    be = e as BinaryExpression;
                    BinaryExpressionType(e as BinaryExpression, " ", query);
                    break;
                case ExpressionType.GreaterThan:
                    query.Append("[");
                    BinaryExpressionType(e as BinaryExpression, ">", query);
                    query.Append("]");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    query.Append("[");
                    BinaryExpressionType(e as BinaryExpression, ">=", query);
                    query.Append("]");
                    break;
                case ExpressionType.LessThan:
                    query.Append("[");
                    BinaryExpressionType(e as BinaryExpression, "<", query);
                    query.Append("]");
                    break;
                case ExpressionType.LessThanOrEqual:
                    query.Append("[");
                    BinaryExpressionType(e as BinaryExpression, ">=", query);
                    query.Append("]");
                    break;
                case ExpressionType.MemberAccess:
                    MemberAccessType(e as MemberExpression, query);
                    break;
                case ExpressionType.Constant:
                    ConstantExpression ce = e as ConstantExpression;
                    if (ce.Type == typeof(DateTime))
                        query.Append(" " + ((DateTime)ce.Value).ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ"));
                    else
                        query.Append(" " + ce.Value);
                    break;
                case ExpressionType.Call:
                    MethodCallExpression(e as MethodCallExpression, query);
                    break;
                case ExpressionType.Convert:
                    ConvertType(e as ConstantExpression);
                    break;
            }
        }
        private void BinaryExpressionType(BinaryExpression e, string oper, StringBuilder query)
        {
            ExpandExpression(e.Left, query);
            query.Append(oper);
            ExpandExpression(e.Right, query);
        }
        private void MemberAccessType(MemberExpression e, StringBuilder query)
        {
            object[] customAttributes = e.Member.GetCustomAttributes(true);
            AttributeItemAttribute itemAttr = (AttributeItemAttribute)customAttributes.FirstOrDefault(attr => attr is AttributeItemAttribute);
            BaseQueryAttribute baseAttr = (BaseQueryAttribute)customAttributes.FirstOrDefault(attr => attr is BaseQueryAttribute);
            if (itemAttr != null)
            {
                string name = itemAttr.Name ?? e.Member.Name;
                if (!string.IsNullOrEmpty(itemAttr.ItemType))
                    query.AppendFormat("{0}({1})", name, itemAttr.ItemType);
                else
                    query.Append(name);
            }
            else
            {
                ExpandExpression(Expression.Constant(Expression.Lambda(e, null).Compile().DynamicInvoke(null)), query);
            }
        }
        private void ConvertType(ConstantExpression e)
        {
        }
        private void MethodCallExpression(MethodCallExpression e, StringBuilder query)
        {
            if (e.Arguments.Count > 0 && e.Arguments[0].NodeType == ExpressionType.Call)
            {
                MethodCallExpression(e.Arguments[0] as MethodCallExpression, query);
            }

            if (e.Object != null)
                ExpandExpression(Expression.Constant(Expression.Lambda(e, null).Compile().DynamicInvoke(null)), query);
            else
            {
                switch (e.Method.Name.ToLower())
                {
                    case "select":
                        break;
                    case "where":
                        Where(e);
                        break;
                    case "orderby":
                        OrderBy(e);
                        SortOrder("ascending");
                        break;
                    case "orderbydescending":
                        OrderBy(e);
                        SortOrder("descending");
                        break;
                    case "take":
                        Take(e);
                        break;
                    case "skip":
                        Skip(e);
                        break;
                    default:
                        throw new Exception("Not supported");
                }
            }
        }
    }
}
