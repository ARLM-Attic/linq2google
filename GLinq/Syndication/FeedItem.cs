using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.ComponentModel;
using GLinq.Mapping;

namespace GLinq.Syndication
{    
    public class FeedItem : SyndicationItem
    {
        protected override bool TryParseElement(System.Xml.XmlReader reader, string version)
        {
            PropertyMapping fpm = this.GetMapping().GetProperty(reader.LocalName, reader.NamespaceURI);                       
            if(fpm != null)
            {
                foreach(TypeConverterAttribute converterAttr in fpm.ConverterAttributes)
                {
                    TypeConverter converter = (TypeConverter)Activator.CreateInstance(Type.GetType(converterAttr.ConverterTypeName));
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        fpm.PropertyInfo.SetValue(this, converter.ConvertFrom(reader.ReadElementContentAsString()), null);
                        return true;
                    }
                }
                fpm.PropertyInfo.SetValue(this, reader.ReadElementContentAs(fpm.PropertyInfo.PropertyType, null), null);
                return true;
            }
            return false;
        }
    }
    
}
