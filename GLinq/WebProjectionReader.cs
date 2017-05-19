using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GLinq
{
    internal class WebProjectionReader<T> : IEnumerable<T>, IEnumerable
    {
        private Enumerator _enumerator;

        public WebProjectionReader(XElement feed, Func<ProjectionRow, T> projector)
        {
            _enumerator = new Enumerator(feed, projector);
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            _enumerator.Reset();
            return _enumerator;
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            _enumerator.Reset();
            return _enumerator;
        }

        #endregion

        private class Enumerator : ProjectionRow, IEnumerator<T>, IEnumerator
        {
            private XElement _feed;
            private int _currentEntry = -1;
            private T _current;
            private Func<ProjectionRow, T> _projector;

            internal Enumerator(XElement feed, Func<ProjectionRow, T> projector)
            {
                _feed = feed;
                _projector = projector;
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get { return _current; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                _currentEntry++;
                if (_feed.Descendants("{http://www.w3.org/2005/Atom}entry").Count() > _currentEntry)
                {
                    _current = _projector(this);
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _currentEntry = -1;
            }

            #endregion

            public override object GetValue(QueryStringParamAttribute descriptor)
            {
                XElement currentEntry = _feed.Descendants("{http://www.w3.org/2005/Atom}entry").ElementAt(_currentEntry);
                ItemAttributeAttribute itemDescriptor = descriptor as ItemAttributeAttribute;
                if (itemDescriptor != null)
                {
                    XElement propElement = currentEntry.Element(XName.Get(itemDescriptor.Name, itemDescriptor.TargetNamespace));
                    return propElement.Value;
                }
                return "hello";
            }
        }
    }
}
