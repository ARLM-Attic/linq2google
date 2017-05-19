using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace GLinq.Mapping
{
    public interface IQueryParser
    {
        string GetQuery(Expression expression);
    }
}
