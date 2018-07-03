using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.TagHelpers
{
    public class CardContext
    {
        public IHtmlContent Title { get; set; }
        public IHtmlContent Body { get; set; }
        public IHtmlContent Footer { get; set; }
    }

}
