using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems
{
    [Feed(BaseUri = "http://www.google.com", UriTemplate = "base/feeds/{FeedType}", DefaultParameterName="bq", Name = "entry", TargetNamespace="http://www.w3.org/2005/Atom")]
    public class Product
    {
        private string _id;
        [ItemAttribute(ElementName = "id", ElementNamespace = "http://www.w3.org/2005/Atom")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _title;
        [ItemAttribute(ElementName = "title", ElementNamespace = "http://www.w3.org/2005/Atom")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _brand;
        [ItemAttribute(ElementName = "brand", ElementNamespace = "http://base.google.com/ns/1.0")]
        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        private float _price;
        [System.ComponentModel.TypeConverter(typeof(FloatUsdConverter))]
        [ItemAttribute(ElementName = "price", ElementNamespace = "http://base.google.com/ns/1.0", ItemType = "float usd", ParameterName="bq")]
        public float Price
        {
            get { return _price; }
            set { _price = value; }
        }

        private string _itemType;
        [ItemAttribute(ElementName = "item_type", ElementNamespace = "http://base.google.com/ns/1.0")]
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        private string _itemLanguage;
        [ItemAttribute(ElementName = "item_language", ElementNamespace = "http://base.google.com/ns/1.0")]
        public string ItemLanguage
        {
            get { return _itemLanguage; }
            set { _itemLanguage = value; }
        }

        private DateTime _expirationDate;
        [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.DateTimeConverter))]
        [ItemAttribute(ElementName = "expiration_date", ElementNamespace = "http://base.google.com/ns/1.0")]
        public DateTime ExpirationDate
        {
            get { return _expirationDate; }
            set { _expirationDate = value; }
        }

        private string _feedType;
        [Url(DefaultValue = "snippets")]
        public string FeedType
        {
            get { return _feedType; }
            set { _feedType = value; }
        }

        private string _bq;
        [QueryStringParam(ParameterName="bq")]
        public string BaseQuery
        {
            get { return _bq; }
            set { _bq = value; }
        }

        internal class FloatUsdConverter : System.ComponentModel.TypeConverter
        {
            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                string floatusd = value as string;
                if (floatusd != null)
                {
                    return float.Parse(floatusd.Replace("usd", ""));
                }
                return base.ConvertFrom(context, culture, value);
            }
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;

                return base.CanConvertFrom(context, sourceType);
            }
        }
    }
}
