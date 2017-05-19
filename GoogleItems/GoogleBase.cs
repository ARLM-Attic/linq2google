using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems
{
    public class GoogleBase
    {
        private string _bq;
        [BaseQuery(ParameterName="bq")]
        public string BaseQuery
        {
            get { return _bq; }
            set { _bq = value; }
        }

        private string _rawEntry;
        public string RawEntry
        {
            get { return _rawEntry; }
            set { _rawEntry = value; }
        }
    }
}
