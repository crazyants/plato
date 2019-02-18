using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
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
            IContextFacade contextFacade,
            IOptions<LocaleOptions> localeOptions)
            : base(localeStore, cacheManager, contextFacade, localeOptions)
        {
        }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

    }

}
