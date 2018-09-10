using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Layout.Localizers
{

    public class LocaleHtmlLocalizer : IHtmlLocalizer
    {

        private string _cultureCode;

        private readonly ILocaleManager _localeManager;
        private readonly ICacheManager _cacheManager;
        private readonly IContextFacade _contextFacade;

        public LocaleHtmlLocalizer(
            ILocaleManager localeManager,
            ICacheManager cacheManager,
            IContextFacade contextFacade)
        {
            _localeManager = localeManager;
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

            var token = _cacheManager.GetOrCreateToken(this.GetType(), _cultureCode);
            return _cacheManager.GetOrCreateAsync(token, async (cacheEntry) =>
            {

                var localizedStrings = new List<LocalizedString>();
                foreach (var localeValue in await _localeManager.GetResourcesAsync<LocaleString>(_cultureCode))
                {
                    localizedStrings.AddRange(localeValue.Values.Select(item =>
                        new LocalizedString(item.Key, item.Value, true)));
                }

                return localizedStrings;

            }).GetAwaiter().GetResult();

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
