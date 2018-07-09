using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Environment = Plato.Internal.Assets.Abstractions.Environment;

namespace Plato.Theming.Assets
{

    public class AssetProvider : IAssetProvider
    {

        private readonly ISiteSettingsStore _siteSettingsStore;

        public AssetProvider(ISiteSettingsStore siteSettingsStore)
        {
            _siteSettingsStore = siteSettingsStore;
        }

        public async Task<IEnumerable<AssetEnvironment>> GetResourceGroups()
        {

            var themeName = await GetThemeNameAsync();
            if (String.IsNullOrEmpty(themeName))
            {
                return null;
            }

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(Environment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/themes/{themeName}/theme.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(Environment.Staging, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url =  $"/themes/{themeName}/theme.min.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                }),

                // Production
                new AssetEnvironment(Environment.Production, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = $"/themes/{themeName}/theme.min.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                })

            };
        }
        
        private async Task<string> GetThemeNameAsync()
        {
            
            var settings = await _siteSettingsStore.GetAsync();
            if (settings != null)
            {
                if (!String.IsNullOrEmpty(settings.ThemeName))
                {
                    return settings.ThemeName.ToLower();
                }
            }

            return string.Empty;

        }


    }

}
