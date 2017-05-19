using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class ItemAttributeAttribute : Attribute
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

    }
}
