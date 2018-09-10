using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Plato.Internal.Cache.Abstractions;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Layout.Localizers
{
    public class LocaleStringLocalizer : IStringLocalizer
    {

        private string _cultureCode;

        private readonly ILocaleManager _localeManager;
        private readonly ICacheManager _cacheManager;
        private readonly IContextFacade _contextFacade;

        public LocaleStringLocalizer(
            ILocaleManager localeManager,
            ICacheManager cacheManager,
            IContextFacade contextFacade1)
        {
            _localeManager = localeManager;
            _cacheManager = cacheManager;
            _contextFacade = contextFacade1;
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

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            _cultureCode = culture.Name;
            return this;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var strings = GetAllStrings(false);
                var value = strings.FirstOrDefault(s => s.Name == name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var strings = GetAllStrings(false);
                var format = strings.FirstOrDefault(s => s.Name == name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }
    }


}
