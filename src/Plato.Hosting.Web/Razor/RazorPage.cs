using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Hosting.Web.Razor
{
    public abstract class RazorPage<TModel> : 
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {    
    
  
    }

    public abstract class RazorPage : RazorPage<dynamic>
    {
    }
}
