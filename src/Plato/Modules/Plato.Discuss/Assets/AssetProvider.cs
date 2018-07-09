using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Discuss.Assets
{
    public class AssetProvider : IAssetProvider
    {
        
        public Task<IEnumerable<AssetEnvironment>> GetResourceGroups()
        {

            IEnumerable<AssetEnvironment> result = new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(Environment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.discuss/content/css/discuss.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Asset()
                    {
                        Url = "/plato.discuss/content/js/discuss.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(Environment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.discuss/content/css/discuss.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Asset()
                    {
                        Url = "/plato.discuss/content/js/discuss.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(Environment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.discuss/content/css/discuss.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Asset()
                    {
                        Url = "/plato.discuss/content/js/discuss.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                    
                })

            };

            return Task.FromResult(result);

        }
        

    }
}
