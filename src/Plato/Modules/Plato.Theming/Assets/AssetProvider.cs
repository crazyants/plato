using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;

namespace Plato.Theming.Assets
{

    public class AssetProvider : IAssetProvider
    {

        private readonly ISiteSettingsStore _siteSettingsStore;

        public AssetProvider(ISiteSettingsStore siteSettingsStore)
        {
            _siteSettingsStore = siteSettingsStore;
        }

        public async Task<IEnumerable<AssetEnvironment>> GetAssetGroups()
        {

            var themeName = await GetThemeNameAsync();
            if (String.IsNullOrEmpty(themeName))
            {
                return null;
            }

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/themes/{themeName}/theme.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url =  $"/themes/{themeName}/theme.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = $"/themes/{themeName}/theme.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
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
