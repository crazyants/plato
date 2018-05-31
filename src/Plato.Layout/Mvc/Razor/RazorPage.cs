using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Layout.Mvc.Razor
{
    public abstract class RazorPage<TModel> :
        Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        
        private IViewLocalizer _t;

        public IViewLocalizer T
        {
            get
            {
                if (_t == null)
                {
                    _t = ViewContext.HttpContext.RequestServices.GetRequiredService<IViewLocalizer>();
                    ((IViewContextAware)_t).Contextualize(this.ViewContext);
                }

                return _t;
            }
        }

   
    }


}
