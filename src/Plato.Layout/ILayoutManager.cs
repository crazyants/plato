using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Layout
{
    public interface ILayoutManager
    {

        Task<IHtmlContent> DisplayAsync(string sectionName);

    }
}
