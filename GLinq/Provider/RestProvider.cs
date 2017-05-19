using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Net;
using System.Xml.Linq;

namespace GLinq.Provider
{
    public class RestProvider : IProvider
    {
        #region IProvider Members

        public IEnumerable<T> Execute<T>(Expression expression, QueryInfo info)
        {
            string query = info.Parser.GetQuery(expression);
            return Execute<T>(query, info);
        }

        private IEnumerable<T> Execute<T>(string url, QueryInfo info)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = info.Method;
            request.ContentType = info.ContentType;
            request.Headers.Add(info.CustomHeaders);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(response.GetResponseStream());
            try
            {
                reader.Read();
                reader.ReadToNextSibling("feed");
                if (!reader.ReadToDescendant("entry"))
                    yield break;
                while (!reader.EOF)
                {
                    object obj = typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
                    XElement entry = XElement.Load(reader.ReadSubtree());
                    foreach (System.Reflection.PropertyInfo prop in typeof(T).GetProperties())
                    {
                        object[] customAttr = prop.GetCustomAttributes(typeof(AttributeItemAttribute), true);
                        if (customAttr.Length == 1)
                        {
                            AttributeItemAttribute itemAttr = customAttr[0] as AttributeItemAttribute;
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
                                    field.SetValue(obj, value);
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
                                    prop.SetValue(obj, value, null);
                                }
                            }
                        }
                    }
                    System.Reflection.PropertyInfo rawEntry = typeof(T).GetProperty("RawEntry");
                    if (rawEntry != null)
                        rawEntry.SetValue(obj, entry.ToString(), null);

                    yield return (T)obj;

                    if (!reader.ReadToNextSibling("entry"))
                        yield break;
                }
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }

        #endregion
    }
}
