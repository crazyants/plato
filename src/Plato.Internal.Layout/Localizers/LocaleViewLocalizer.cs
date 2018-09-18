using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Layout.Localizers
{

    public class LocaleViewLocalizer :
        LocaleHtmlLocalizer,
        IViewLocalizer,
        IViewContextAware
    {
        public ViewContext ViewContext;

        public LocaleViewLocalizer(
            ILocaleStore localeStore,
            ICacheManager cacheManager,
            IContextFacade contextFacade)
            : base(localeStore, cacheManager, contextFacade)
        {
        }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

    }

}
