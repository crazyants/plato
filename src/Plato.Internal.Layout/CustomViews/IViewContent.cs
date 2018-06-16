using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.CustomViews
{
    public interface IViewContent
    {

        IHtmlContent Output { get; }

    }

}
