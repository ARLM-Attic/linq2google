using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

namespace GLinq
{
    internal class AtomFeedSerializer
    {
        public static IEnumerable<T> Deserialize<T>(XmlReader reader, QueryInfo info)
        {
            List<T> list = new List<T>();

            XElement feed = XDocument.Load(reader).Element("{http://www.w3.org/2005/Atom}feed");
            if (feed == null)
                throw new Exception("Feed not found");

            if (info is FeedInfo<T>)
                ParseFeed(feed, info as FeedInfo<T>);

            foreach (XElement entry in feed.Descendants("{http://www.w3.org/2005/Atom}entry"))
            {
                object entryObj = Activator.CreateInstance(typeof(T));

                foreach (System.Reflection.PropertyInfo prop in typeof(T).GetProperties())
                {
                    object[] customAttr = prop.GetCustomAttributes(typeof(ItemAttributeAttribute), true);
                    if (customAttr.Length == 1)
                    {
                        ItemAttributeAttribute itemAttr = customAttr[0] as ItemAttributeAttribute;
                        string targetNamespace = itemAttr.TargetNamespace ?? "";
                        string name = itemAttr.Name ?? prop.Name;
                        XElement propElement = entry.Element(XName.Get(name, targetNamespace));
                        if (propElement != null)
                        {
                            object value = null;
                            if (!String.IsNullOrEmpty(itemAttr.Storage))
                            {
                                System.Reflection.FieldInfo field = typeof(T).GetField(itemAttr.Storage, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                                if (field.FieldType == typeof(string))
                                    value = propElement.Value;
                                else
                                {
                                    System.Reflection.MethodInfo mi = field.FieldType.GetMethod("Parse", new Type[] { typeof(string) });
                                    if (mi != null)
                                        value = mi.Invoke(null, new object[] { propElement.Value });
                                    else
                                        throw new Exception("No parse method available from string to type " + field.FieldType.Name);
                                }
                                field.SetValue(entryObj, value);
                            }
                            else
                            {
                                if (prop.PropertyType == typeof(string))
                                    value = propElement.Value;
                                else
                                {
                                    System.Reflection.MethodInfo mi = prop.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                                    if (mi != null)
                                        value = mi.Invoke(null, new object[] { propElement.Value });
                                    else
                                        throw new Exception("No parse method available from string to type " + prop.PropertyType.Name);
                                }
                                prop.SetValue(entryObj, value, null);
                            }
                        }
                    }
                }
                System.Reflection.PropertyInfo rawEntry = typeof(T).GetProperty("RawEntry");
                if (rawEntry != null)
                    rawEntry.SetValue(entryObj, entry.ToString(), null);

                list.Add((T)entryObj);
            }
            return list;
        }

        private static void ParseFeed<T>(XElement feed, FeedInfo<T> feedInfo)
        {
            feedInfo.Feed.Id = feed.Element("{http://www.w3.org/2005/Atom}id").Value;            
            feedInfo.Feed.TotalResults = int.Parse(feed.Element("{http://a9.com/-/spec/opensearchrss/1.0/}totalResults").Value);

            if (feed.Element("{http://www.w3.org/2005/Atom}updated") != null)
                feedInfo.Feed.Updated = DateTime.ParseExact(feed.Element("{http://www.w3.org/2005/Atom}updated").Value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFF'Z'", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }
    }
}
