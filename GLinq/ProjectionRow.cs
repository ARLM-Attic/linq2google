using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public abstract class ProjectionRow
    {
        public abstract object GetValue(QueryStringParamAttribute descriptor);
    }
}
