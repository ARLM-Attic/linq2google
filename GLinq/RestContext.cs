using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class RestContext
    {
        private IProvider _provider;
        public IProvider Provider
        {
            get { return _provider; }
            protected set { _provider = value; }
        }

        public RestContext()
        {
            _provider = new RestProvider();
        }
    }
}
