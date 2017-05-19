using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GLinq.Mapping
{
    public class QueryMapping
    {
        private List<PropertyMapping> _properties;
        public ReadOnlyCollection<PropertyMapping> Properties
        {
            get { return _properties.AsReadOnly(); }
        }

        private Type _type;
        public Type Type
        {
            get { return _type; }
        }

        public PropertyMapping GetProperty(string localName, string nameSpace)
        {
            return _properties.Where(x => x.ElementName.ToLower() == localName.ToLower() && x.ElementNamespace.ToLower() == nameSpace.ToLower()).FirstOrDefault();
        }

        public QueryMapping(Type type, List<PropertyMapping> properties)
        {
            _properties = properties;
            _type = type;
        }
    }
}
