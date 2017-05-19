using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GLinq.Mapping
{
    public static class QueryMappingFactory
    {
        private static Dictionary<Type, QueryMapping> maps = new Dictionary<Type, QueryMapping>();

        public static QueryMapping GetMapping(this object item)
        {
            Type type = item.GetType();
            if (!maps.ContainsKey(type))
                maps[type] = CreateMap(type);

            return maps[type];
        }
        private static QueryMapping CreateMap(Type type)
        {
            List<PropertyMapping> props = new List<PropertyMapping>();
            foreach (PropertyInfo pi in type.GetProperties())
            {
                PropertyMapping prop = new PropertyMapping(pi);
                props.Add(prop);
            }

            return new QueryMapping(type, props);
        }
    }
}
