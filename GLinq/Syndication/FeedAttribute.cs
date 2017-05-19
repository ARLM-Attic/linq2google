using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public class FeedAttribute : Attribute
    {
        private string _uriTemplate;
        public string UriTemplate
        {
            get { return _uriTemplate; }
            set { _uriTemplate = value; }
        }
        private string _baseUri;
        public string BaseUri
        {
            get { return _baseUri; }
            set { _baseUri = value; }
        }
        
        private string _defaultParameterName = "";
        public string DefaultParameterName
        {
            get { return _defaultParameterName; }
            set { _defaultParameterName = value; }
        }


        public Dictionary<string, StringBuilder> FormatSortBy(string value, SortDirection direction)
        {
            Dictionary<string, StringBuilder> parameters = new Dictionary<string, StringBuilder>();
            parameters.Add("orderby", new StringBuilder(value));
            if (direction == SortDirection.Ascending)
                parameters.Add("sortorder", new StringBuilder("ascending"));
            else
                parameters.Add("sortorder", new StringBuilder("descending"));
            return parameters;
        }
    }
}
