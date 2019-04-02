using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Core.Assets
{

    public class AssetProvider : IAssetProvider
    {

        private readonly IContextFacade _contextFacade;

        public AssetProvider(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public async Task<IEnumerable<AssetEnvironment>> GetAssetEnvironments()
        {

            var theme = await _contextFacade.GetCurrentThemeAsync();
            if (String.IsNullOrEmpty(theme))
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
                        Url = $"/{theme}/theme.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Order = int.MaxValue
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url =  $"/{theme}/theme.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Order = int.MaxValue
                    },
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = $"/{theme}/theme.min.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header,
                        Order = int.MaxValue
                    },
                })

            };
        }
        


    }

}
