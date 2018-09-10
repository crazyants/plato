using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Layout.Localizers
{

    public class LocaleHtmlLocalizer : IHtmlLocalizer
    {

        private string _cultureCode;

        private readonly ILocaleStore _localeStore;
        private readonly ICacheManager _cacheManager;
        private readonly IContextFacade _contextFacade;

        public LocaleHtmlLocalizer(
            ILocaleStore localeStore,
            ICacheManager cacheManager,
            IContextFacade contextFacade)
        {
            _localeStore = localeStore;
            _cacheManager = cacheManager;
            _contextFacade = contextFacade;
        }
        
        public LocalizedString GetString(string name)
        {
            var strings = GetAllStrings(false);
            var value = strings.FirstOrDefault(s => s.Name == name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }

        public LocalizedString GetString(string name, params object[] arguments)
        {
            var strings = GetAllStrings(false);
            var format = strings.FirstOrDefault(s => s.Name == name);
            var value = string.Format(format ?? name, arguments);
            return new LocalizedString(name, value, resourceNotFound: format == null);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {

            if (String.IsNullOrEmpty(_cultureCode))
            {
                _cultureCode = _contextFacade.GetCurrentCulture();
            }

            return _localeStore.GetAllStringsAsync(_cultureCode)
                .GetAwaiter()
                .GetResult();

        }

        public IHtmlLocalizer WithCulture(CultureInfo culture)
        {
            _cultureCode = culture.Name;
            return this;
        }

        public LocalizedHtmlString this[string name]
        {
            get
            {
                var strings = GetAllStrings(false);
                var value = strings.FirstOrDefault(s => s.Name == name);
                return new LocalizedHtmlString(name, value ?? name);
            }
        }

        public LocalizedHtmlString this[string name, params object[] arguments]
        {
            get
            {
                var strings = GetAllStrings(false);
                var format = strings.FirstOrDefault(s => s.Name == name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedHtmlString(name, value);
            }
        }
    }
}
