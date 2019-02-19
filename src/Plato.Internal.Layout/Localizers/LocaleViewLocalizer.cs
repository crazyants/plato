using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;

namespace Plato.Internal.Layout.Localizers
{

    public class LocaleViewLocalizer :
        LocaleHtmlLocalizer,
        IViewLocalizer,
        IViewContextAware
    {
        public ViewContext ViewContext { get; private set; }

        public LocaleViewLocalizer(
            ILocaleStore localeStore,
            IOptions<LocaleOptions> localeOptions)
            : base(localeStore,  localeOptions)
        {
        }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

    }

}
