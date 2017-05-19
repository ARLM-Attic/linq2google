using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GLinq
{
    public interface IResponseItemAttribute
    {
        string ElementName
        {
            get;set;
        }
        string ElementNamespace
        {
            get;set;
        }
    }
}
