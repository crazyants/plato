using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Resources.Abstractions;
using Plato.Internal.Stores.Abstractions.Settings;
using Environment = Plato.Internal.Resources.Abstractions.Environment;

namespace Plato.Theming.Resources
{
    public class ResourceProvider : IResourceProvider
    {

        private readonly ISiteSettingsStore _siteSettingsStore;

        public ResourceProvider(ISiteSettingsStore siteSettingsStore)
        {
            _siteSettingsStore = siteSettingsStore;
        }

        public async Task<IEnumerable<ResourceGroup>> GetResourceGroups()
        {

            var themeName = await GetThemeNameAsync();
            if (String.IsNullOrEmpty(themeName))
            {
                return null;
            }

            return new List<ResourceGroup>
            {

                // Development
                new ResourceGroup(Environment.Development, new List<Resource>()
                {
                    new Resource()
                    {
                        Url = $"/themes/{themeName}/theme.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                }),

                // Staging
                new ResourceGroup(Environment.Staging, new List<Resource>()
                {
                    /* Css */
                    new Resource()
                    {
                        Url =  $"/themes/{themeName}/theme.min.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                }),

                // Production
                new ResourceGroup(Environment.Production, new List<Resource>()
                {
                    /* Css */
                    new Resource()
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
