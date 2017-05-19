using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class RestContext
    {
        private Provider.IProvider _provider;
        public Provider.IProvider Provider
        {
            get { return _provider; }
            protected set { _provider = value; }
        }

        public RestContext()
        {
            _provider = new Provider.RestProvider();
        }
    }
}
