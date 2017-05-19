using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems
{
    public abstract class AtomEntry
    {
        private string _id;
        [ItemAttribute(Name = "id", TargetNamespace = "http://www.w3.org/2005/Atom")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _title;
        [ItemAttribute(Name = "title", TargetNamespace = "http://www.w3.org/2005/Atom")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
    }
}
