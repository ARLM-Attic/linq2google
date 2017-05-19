using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GLinq.Mapping
{
    public class QueryMapping
    {
        private List<PropertyMapping> _properties = new List<PropertyMapping>();
        public ReadOnlyCollection<PropertyMapping> Properties
        {
            get { return _properties.AsReadOnly(); }
        }

        private Type _type;
        public Type Type
        {
            get { return _type; }
        }

        private FeedAttribute _feed;
        public FeedAttribute Feed
        {
            get { return _feed; }            
        }

        public string ElementName
        {
            get
            {
                if(Feed != null && !String.IsNullOrEmpty(Feed.Name))
                    return Feed.Name;
                return _type.Name;
            }
        }
        public string TargetNamespace
        {
            get
            {
                if(Feed != null && !String.IsNullOrEmpty(Feed.TargetNamespace))
                    return Feed.TargetNamespace;
                return "";
            }
        }

        public PropertyMapping GetProperty(string localName, string nameSpace)
        {
            return _properties.Where(x => x.ElementName.ToLower() == localName.ToLower() && x.ElementNamespace.ToLower() == nameSpace.ToLower()).FirstOrDefault();
        }
        public PropertyMapping GetProperty(MemberInfo info)
        {
            return _properties.Where(x => x.PropertyInfo == info).FirstOrDefault();
        }


        public QueryMapping(Type type)
        {
            _type = type;
            _feed = type.GetCustomAttributes(typeof(FeedAttribute), true).Cast<FeedAttribute>().FirstOrDefault<FeedAttribute>();
            while(type != typeof(object))
            {
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    PropertyMapping prop = new PropertyMapping(this, pi);
                    _properties.Add(prop);
                }
                type = type.BaseType;
            }
        }
    }
}
