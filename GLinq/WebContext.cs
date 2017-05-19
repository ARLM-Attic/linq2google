using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class WebContext
    {
        private IProvider _provider;
        public IProvider Provider
        {
            get { return _provider; }
            protected set { _provider = value; }
        }

        public WebContext()
        {
            _provider = new WebProvider();
        }
    }
}
