using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace GLinq
{
    public class QueryStringParamAttribute : Attribute
    {
        private string _name;
        public virtual string ParameterName
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string FormatOrderByName()
        {
            return ParameterName;
        }
        public virtual string FormatQueryStringItem(string value, ExpressionType operType)
        {
            return value;
        }

    }
}
