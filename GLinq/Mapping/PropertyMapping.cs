using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Reflection;

namespace GLinq.Mapping
{
    public class PropertyMapping
    {
        private List<XmlElementAttribute> _xmlAttrs;
        public ReadOnlyCollection<XmlElementAttribute> XmlAttributes
        {
            get { return _xmlAttrs.AsReadOnly(); }
        }
        private List<ItemAttributeAttribute> _itemAttrs;
        public ReadOnlyCollection<ItemAttributeAttribute> ItemAttributes
        {
            get { return _itemAttrs.AsReadOnly(); }
        }
        private List<TypeConverterAttribute> _converterAttrs;
        public ReadOnlyCollection<TypeConverterAttribute> ConverterAttributes
        {
            get { return _converterAttrs.AsReadOnly(); }
        }

        private PropertyInfo _pi;
        public PropertyInfo PropertyInfo
        {
            get { return _pi; }
        }

        public string ElementName
        {
            get
            {
                if (_itemAttrs.Count > 0)
                {
                    return _itemAttrs[0].Name;
                }
                else if (_xmlAttrs.Count > 0)
                {
                    return _xmlAttrs[0].ElementName;
                }
                else
                    return _pi.Name;
            }
        }
        public string ElementNamespace
        {
            get
            {
                if (_itemAttrs.Count > 0)
                {
                    return _itemAttrs[0].TargetNamespace;
                }
                else if (_xmlAttrs.Count > 0)
                {
                    return _xmlAttrs[0].Namespace;
                }
                else
                    return "";
            }
        }

        public PropertyMapping(PropertyInfo pi)
        {
            _xmlAttrs = new List<XmlElementAttribute>(pi.GetCustomAttributes(typeof(XmlElementAttribute), true).Cast<XmlElementAttribute>());
            _itemAttrs = new List<ItemAttributeAttribute>(pi.GetCustomAttributes(typeof(ItemAttributeAttribute), true).Cast<ItemAttributeAttribute>());
            _converterAttrs = new List<TypeConverterAttribute>(pi.GetCustomAttributes(typeof(TypeConverterAttribute), true).Cast<TypeConverterAttribute>());
            _pi = pi;
        }
    }
}
