using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    public class ItemAttributeAttribute : QueryStringParamAttribute
    {
        private string _itemType;
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        private string _targetNamespace;
        public string TargetNamespace
        {
            get { return _targetNamespace; }
            set { _targetNamespace = value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _useField;
        public string Storage
        {
            get { return _useField; }
            set { _useField = value; }
        }
        
        public override string FormatQueryStringItem(string value, ExpressionType operType)
        {
            string name = this.Name;
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
