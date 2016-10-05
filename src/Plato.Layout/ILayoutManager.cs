using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout
{
    public interface ILayoutManager
    {

        ViewContext ViewContext { get; set;  }

        IHtmlContent Display(string sectionName, dynamic arguments);

        Task<IHtmlContent> DisplayAsync(string sectionName, dynamic arguments);

    }
}
