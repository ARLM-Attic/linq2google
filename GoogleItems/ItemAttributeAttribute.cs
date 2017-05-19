using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using GLinq;

namespace GoogleItems
{
    public class ItemAttributeAttribute : QueryStringParamAttribute, IResponseItemAttribute
    {
        private string _itemType;
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        private string _targetNamespace;
        public string ElementNamespace
        {
            get { return _targetNamespace; }
            set { _targetNamespace = value; }
        }

        private string _name;
        public string ElementName
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string FormatOrderByName()
        {
            string name = this.ElementName;
            if (!String.IsNullOrEmpty(this.ItemType))
                name += "(" + ItemType + ")";
            return name;
        }
        public override string FormatQueryStringItem(string value, ExpressionType operType)
        {
            string name = this.ElementName;
            if (!String.IsNullOrEmpty(this.ItemType))
                name += "(" + ItemType + ")";
            
            return "[" + name + GetOperator(operType) + value + "]";
        }
        protected virtual string GetOperator(ExpressionType operType)
        {
            switch (operType)
            {
                case ExpressionType.Equal:
                    return ": ";
                case ExpressionType.NotEqual:
                    return " != ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", operType));
            }
        }
    }
}
