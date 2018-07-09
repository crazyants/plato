using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Entities.Follow.Assets
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
                        Url = "/plato.entities.follow/content/js/follow.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(Environment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.entities.follow/content/js/follow.min.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(Environment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.entities.follow/content/js/follow.min.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                })

            };

            return Task.FromResult(result);

        }
        

    }
}
