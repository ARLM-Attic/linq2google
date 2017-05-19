using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace GLinq
{
    public class FeedParser : IQueryParser
    {
        private Dictionary<string, string> _urlReplacers = new Dictionary<string, string>();
        private Dictionary<string, StringBuilder> _queryString = new Dictionary<string, StringBuilder>();
        private StringBuilder _baseQuery = new StringBuilder();
        private StringBuilder _orderbyQuery = new StringBuilder();
        private StringBuilder _sortQuery = new StringBuilder();
        private int _takeQuery = -1;
        private int _skipQuery = -1;
        private string _baseURL;

        public FeedParser(string baseURL, Type queryable)
        {
            _baseURL = baseURL;
            SetUrlReplacerDefaults(queryable);
        }

        private void SetUrlReplacerDefaults(Type queryable)
        {
            foreach (System.Reflection.PropertyInfo prop in queryable.GetProperties())
            {
                object[] attrs = prop.GetCustomAttributes(typeof(UrlAttribute), true);
                if (attrs.Length == 1)
                {
                    _urlReplacers[prop.Name] = ((UrlAttribute)attrs[0]).DefaultValue;
                }
            }
        }

        #region IQueryParser Members

        public string GetQuery(Expression expression)
        {
            ExpandExpression(expression, null);
            
            string query = "";
            foreach (KeyValuePair<string, StringBuilder> keyValue in _queryString)
            {
                if (keyValue.Value != null && keyValue.Value.Length > 0)
                    query += String.Format("{0}{1}={2}", (query.Length > 0 ? "&" : ""), keyValue.Key, keyValue.Value.ToString());
            }

            foreach (KeyValuePair<string, string> keyValue in _urlReplacers)
                _baseURL = _baseURL.Replace(keyValue.Key, keyValue.Value);

            if (query.Length > 0)
                return _baseURL + "?" + query;
            else
                return _baseURL;
        }

        #endregion
        
        private void Where(MethodCallExpression method)
        {
            LambdaExpression lamda = ((UnaryExpression)method.Arguments[1]).Operand as LambdaExpression;
            ExpandExpression(lamda.Body, null);
        }
        private void OrderBy(MethodCallExpression method)
        {
            StringBuilder tempQuery = new StringBuilder();
            LambdaExpression lamda = ((UnaryExpression)method.Arguments[1]).Operand as LambdaExpression;
            ExpandExpression(lamda.Body, tempQuery);

            if (_queryString.ContainsKey("orderby") && _queryString["orderby"].ToString() != tempQuery.ToString())
                throw new Exception("OrderBy can only be performed on a single attribute");
            
            _queryString["orderby"] = tempQuery;
        }
        private void SortOrder(string order)
        {
            _queryString["sortorder"] = new StringBuilder(order);
        }
        private void Take(MethodCallExpression method)
        {
            int take = (int)((ConstantExpression)method.Arguments[1]).Value;
            _queryString["max-results"] = new StringBuilder(take.ToString());
        }
        private void Skip(MethodCallExpression method)
        {
            int skip = (int)((ConstantExpression)method.Arguments[1]).Value;
            _queryString["start-index"] = new StringBuilder(skip.ToString());
        }

        private void ExpandExpression(Expression e, StringBuilder query)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    BinaryOperation(e as BinaryExpression);
                    break;
                case ExpressionType.OrElse:
                    BinaryExpression be = e as BinaryExpression;
                    query.Append("(");
                    BinaryExpressionType(e as BinaryExpression);
                    query.Append(")");
                    break;
                case ExpressionType.AndAlso:
                    be = e as BinaryExpression;
                    BinaryExpressionType(e as BinaryExpression);
                    break;
                case ExpressionType.MemberAccess:
                    MemberAccessType(e as MemberExpression, query);
                    break;
                case ExpressionType.Constant:
                    ReadConstantExpression(e as ConstantExpression, query);
                    break;
                case ExpressionType.Call:
                    MethodCallExpression(e as MethodCallExpression, query);
                    break;
                case ExpressionType.Convert:
                    ConvertType(e as ConstantExpression);
                    break;
            }
        }
        private void BinaryOperation(BinaryExpression be)
        {
            MemberExpression left = be.Left as MemberExpression;
            if (left == null)
                throw new Exception("The left hand side of the expression must be a queryable member");
            object[] customAttributes = left.Member.GetCustomAttributes(true);
            if (customAttributes.Length != 1)
                throw new Exception("The property " + left.Member.Name + " must be marked as queryable for use in a Query Expression");
        
            if (customAttributes[0] is BaseQueryAttribute)
            {
                string parameter = GetParameterName(customAttributes[0] as QueryStringParamAttribute, left.Member);
                if (!_queryString.ContainsKey(parameter))
                    _queryString[parameter] = new StringBuilder();
                ExpandExpression(be.Right, _queryString[parameter]);
            }
            else if (customAttributes[0] is ItemAttributeAttribute)
            {
                string parameter = GetParameterName(customAttributes[0] as QueryStringParamAttribute, left.Member);
                if (!_queryString.ContainsKey(parameter))
                    _queryString[parameter] = new StringBuilder();
                _queryString[parameter].Append(" [");
                ExpandExpression(be.Left, _queryString[parameter]);
                _queryString[parameter].Append(GetOperator(be, (Attribute)customAttributes[0]));
                ExpandExpression(be.Right, _queryString[parameter]);
                _queryString[parameter].Append("]");
            }
            else if (customAttributes[0] is UrlAttribute)
            {
                _urlReplacers[left.Member.Name] = ((ConstantExpression)be.Right).Value.ToString();
            }
        }
        private void BinaryExpressionType(BinaryExpression e)
        {
            string param = GetConsistantQueryParam(e);
            if (!_queryString.ContainsKey(param))
                _queryString[param] = new StringBuilder();

            ExpandExpression(e.Left, _queryString[param]);
            _queryString[param].Append(GetOperator(e, null));
            ExpandExpression(e.Right, _queryString[param]);
        }
        private void MemberAccessType(MemberExpression e, StringBuilder query)
        {
            object[] customAttributes = e.Member.GetCustomAttributes(true);
            ItemAttributeAttribute itemAttr = (ItemAttributeAttribute)customAttributes.FirstOrDefault(attr => attr is ItemAttributeAttribute);
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
            if(e.Object != null)
                ExpandExpression(Expression.Constant(Expression.Lambda(e, null).Compile().DynamicInvoke(null)), query);
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
        private void ReadConstantExpression(ConstantExpression ce, StringBuilder query)
        {
            if (ce.Type == typeof(string))
                query.Append(" " + ce.Value);
            else if (ce.Type == typeof(DateTime))
                query.Append(((DateTime)ce.Value).ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ"));
            else
                query.Append(ce.Value.ToString());
        }

        private string GetConsistantQueryParam(BinaryExpression be)
        {
            string leftName = "";

            if (be.Left.NodeType == ExpressionType.MemberAccess)
                 return GetParameterName(null, ((MemberExpression)be.Left).Member);
            else if (be.Left is BinaryExpression)
                leftName = GetConsistantQueryParam(be.Left as BinaryExpression);

            string rightName = "";
            if (be.Right.NodeType == ExpressionType.MemberAccess)
                return GetParameterName(null, ((MemberExpression)be.Right).Member);
            if (be.Right is BinaryExpression)
                rightName = GetConsistantQueryParam(be.Right as BinaryExpression);

            if (leftName == rightName)
                return leftName;

            throw new Exception("The operator " + be.NodeType.ToString() + " cannot be used with properties having different ParameterName values.  Use multiple Where clauses instead.");
        }
        private string GetOperator(BinaryExpression be, Attribute propertyAttr)
        {
            switch (be.NodeType)
            {
                case ExpressionType.Equal:
                    return ":";
                case ExpressionType.NotEqual:
                    if (propertyAttr is BaseQueryAttribute || propertyAttr is UrlAttribute)
                        return "-";
                    else if (propertyAttr is ItemAttributeAttribute)
                        return "!=";
                    break;
                case ExpressionType.OrElse:
                    return "|";
                case ExpressionType.AndAlso:
                    return  " ";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
            }
            return "";
        }
        private string GetParameterName(QueryStringParamAttribute attr, System.Reflection.MemberInfo mi)
        {
            string parameterName = null;
            object[] customAttr;
            if (attr == null)
            {
                customAttr = mi.GetCustomAttributes(typeof(QueryStringParamAttribute), true);
                if (customAttr.Length == 1)
                    parameterName = ((QueryStringParamAttribute)customAttr[0]).ParameterName;
            }
            else
            {
                parameterName = attr.ParameterName;
            }

            if (!string.IsNullOrEmpty(parameterName))
                return parameterName;

            customAttr = mi.DeclaringType.GetCustomAttributes(typeof(FeedAttribute), true);
            if (customAttr.Length == 1)
                return ((FeedAttribute)customAttr[0]).DefaultParameterName;

            return null;
        }
    }
}
