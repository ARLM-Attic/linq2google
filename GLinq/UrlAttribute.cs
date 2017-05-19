using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class UrlAttribute : Attribute
    {
        private string _defaultValue = "";
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
    }
}
