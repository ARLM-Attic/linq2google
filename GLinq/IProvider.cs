using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace GLinq
{
    public interface IProvider
    {
        object Execute(Expression expression, QueryInfo info);
        string GetQueryText(Expression expression);
    }
}
