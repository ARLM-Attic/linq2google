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
        private QueryMapping _owner;
        public QueryMapping Owner
        {
            get{ return _owner; }
        }
        private List<XmlElementAttribute> _xmlAttrs;
        public ReadOnlyCollection<XmlElementAttribute> XmlAttributes
        {
            get { return _xmlAttrs.AsReadOnly(); }
        }
        private List<QueryStringParamAttribute> _queryAttrs;
        public ReadOnlyCollection<QueryStringParamAttribute> ParamAttributes
        {
            get { return _queryAttrs.AsReadOnly(); }
        }
        public ReadOnlyCollection<IResponseItemAttribute> ResponseItemAttributes
        {
            get { return new ReadOnlyCollection<IResponseItemAttribute>(_queryAttrs.OfType<IResponseItemAttribute>().ToList()); }
        }
        private List<UrlAttribute> _urlAttrs;
        public ReadOnlyCollection<UrlAttribute> UrlAttributes
        {
            get { return _urlAttrs.AsReadOnly(); }
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

        public string UrlVariableName
        {
            get
            {
                if(_urlAttrs.Count == 0)
                    return "";
                else if(String.IsNullOrEmpty(_urlAttrs[0].Name))
                    return _pi.Name;
                    
                return _urlAttrs[0].Name;
            }
        }
        public string ParameterName
        {
            get
            {
                if(_queryAttrs.Count == 0)
                    return "";
                else if(String.IsNullOrEmpty(_queryAttrs[0].ParameterName))
                {
                    if(String.IsNullOrEmpty(Owner.Feed.DefaultParameterName))
                        return _pi.Name;

                    return Owner.Feed.DefaultParameterName;
                }

                return _queryAttrs[0].ParameterName;
            }
        }
        public string ElementName
        {
            get
            {
                if(ResponseItemAttributes.Count > 0)
                    return ResponseItemAttributes[0].ElementName; 
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
                if(ResponseItemAttributes.Count > 0)
                    return ResponseItemAttributes[0].ElementNamespace; 
                else if (_xmlAttrs.Count > 0)
                    return _xmlAttrs[0].Namespace;
                else if(!String.IsNullOrEmpty(Owner.Feed.TargetNamespace))
                    return Owner.Feed.TargetNamespace;
                else
                    return "";
            }
        }

        public PropertyMapping(QueryMapping owner, PropertyInfo pi)
        {
            _owner = owner;
            _xmlAttrs = new List<XmlElementAttribute>(pi.GetCustomAttributes(typeof(XmlElementAttribute), true).Cast<XmlElementAttribute>());
            _queryAttrs = new List<QueryStringParamAttribute>(pi.GetCustomAttributes(typeof(QueryStringParamAttribute), true).Cast<QueryStringParamAttribute>());
            _urlAttrs = new List<UrlAttribute>(pi.GetCustomAttributes(typeof(UrlAttribute), true).Cast<UrlAttribute>());
            _converterAttrs = new List<TypeConverterAttribute>(pi.GetCustomAttributes(typeof(TypeConverterAttribute), true).Cast<TypeConverterAttribute>());
            _pi = pi;
        }
    }
}
