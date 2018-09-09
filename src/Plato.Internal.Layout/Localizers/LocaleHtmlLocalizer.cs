using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;

namespace Plato.Internal.Layout.Localizers
{
    
    public class LocaleHtmlLocalizer : IHtmlLocalizer
    {
        public LocalizedString GetString(string name)
        {
            throw new NotImplementedException();
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public LocalizedHtmlString this[string name] => throw new NotImplementedException();

        public LocalizedHtmlString this[string name, params object[] arguments] => throw new NotImplementedException();
    }
}
