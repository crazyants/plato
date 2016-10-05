using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout.Elements
{
    public class ElementMetaData
    {

        public string Type { get; set; }
        public string DisplayType { get; set; }
        public string Position { get; set; }
        public IHtmlContent ChildContent { get; set; }


    }
}
