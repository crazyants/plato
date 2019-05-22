using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Core.Configuration
{

    public class SiteOptionsConfiguration : IConfigureOptions<SiteOptions>
    {

        private readonly ISiteSettingsStore _siteSettingsStore;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SiteOptionsConfiguration(
            IServiceScopeFactory serviceScopeFactory,
            ISiteSettingsStore siteSettingsStore)
        {
         
            _serviceScopeFactory = serviceScopeFactory;
            _siteSettingsStore = siteSettingsStore;
        }

        public void Configure(SiteOptions options)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {

                var settings = _siteSettingsStore
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();

                if (settings != null)
                {
                    options.SiteName = settings.SiteName;
                    options.Culture = settings.Culture;
                    options.DateTimeFormat = settings.DateTimeFormat;
                    options.Theme = settings.Theme;
                }
            
            }

        }

    }

}
