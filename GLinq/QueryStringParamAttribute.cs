using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class QueryStringParamAttribute : Attribute
    {
        private string _name;
        public string ParameterName
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
