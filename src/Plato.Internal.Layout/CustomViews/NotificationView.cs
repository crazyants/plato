using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.CustomViews
{


    
    public class NotificationView : IViewContent
    {
        
        public NotificationView(string html)
        {
            Output = new HtmlString(html);
        }


        public IHtmlContent Output { get; }

    }
}
