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

        public static QueryMapping GetMapping(this Type item)
        {
            if (!maps.ContainsKey(item))
                maps[item] = new QueryMapping(item);

            return maps[item];
        }
    }
}
