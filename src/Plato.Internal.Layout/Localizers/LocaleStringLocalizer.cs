using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Plato.Internal.Localization.Abstractions;
using Plato.Internal.Localization.Abstractions.Models;
using LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace Plato.Internal.Layout.Localizers
{
    public class LocaleStringLocalizer : IStringLocalizer
    {

        private string _cultureCode;

        private readonly ILocaleStore _localeStore;
        private readonly IOptions<LocaleOptions> _localeOptions;

        public LocaleStringLocalizer(
            ILocaleStore localeStore,
            IOptions<LocaleOptions> localeOptions)
        {
            _localeStore = localeStore;
            _localeOptions = localeOptions;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            
            if (String.IsNullOrEmpty(_cultureCode))
            {
                _cultureCode = _localeOptions.Value.Culture;
            }

            // TODO: Remove GetAwaiter IStringLocalizer.GetAllStrings is not async
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
