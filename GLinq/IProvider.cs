﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace GLinq
{
    public interface IProvider
    {
         IEnumerable<T> Execute<T>(Expression expression, QueryInfo info);
    }
}