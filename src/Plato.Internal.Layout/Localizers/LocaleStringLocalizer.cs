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

        private readonly ILocaleStore _localeStore;
        private readonly ICacheManager _cacheManager;
        private readonly IContextFacade _contextFacade;

        public LocaleStringLocalizer(
            ILocaleStore localeStore,
            ICacheManager cacheManager,
            IContextFacade contextFacade1)
        {
            _localeStore = localeStore;
            _cacheManager = cacheManager;
            _contextFacade = contextFacade1;
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
