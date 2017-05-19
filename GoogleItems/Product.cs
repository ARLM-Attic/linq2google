using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GLinq;

namespace GoogleItems
{
    [Feed(BaseURL = "http://www.google.com/base/feeds/FeedType", DefaultParameterName="bq")]
    public class Product : GoogleBase
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

        private string _brand;
        [ItemAttribute(Name = "brand", TargetNamespace = "http://base.google.com/ns/1.0")]
        public string Brand
        {
            get { return _brand; }
            set { _brand = value; }
        }

        private string _price;
        [ItemAttribute(Name = "price", TargetNamespace = "http://base.google.com/ns/1.0", Storage = "_price", ItemType = "float usd", ParameterName="bq")]
        public float Price
        {
            get { return float.Parse(_price.Replace("usd", "")); }
            set { _price = value.ToString(); }
        }

        private string _itemType;
        [ItemAttribute(Name = "item_type", TargetNamespace = "http://base.google.com/ns/1.0")]
        public string ItemType
        {
            get { return _itemType; }
            set { _itemType = value; }
        }

        private string _itemLanguage;
        [ItemAttribute(Name = "item_language", TargetNamespace = "http://base.google.com/ns/1.0")]
        public string ItemLanguage
        {
            get { return _itemLanguage; }
            set { _itemLanguage = value; }
        }

        private DateTime _expirationDate;
        [ItemAttribute(Name = "expiration_date", TargetNamespace = "http://base.google.com/ns/1.0")]
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
    }
}
