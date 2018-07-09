using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Markdown.Assets
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
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/init.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/markdown.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                }),

                // Staging
                new AssetEnvironment(Environment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/init.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/markdown.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                }),

                // Production
                new AssetEnvironment(Environment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/js/init.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = $"/plato.markdown/content/css/markdown.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                })

            };

            return Task.FromResult(result);

        }


    }
}
