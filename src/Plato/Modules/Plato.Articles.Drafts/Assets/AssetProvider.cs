using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Articles.Drafts.Assets
{
    public class AssetProvider : IAssetProvider
    {
        
        public Task<IEnumerable<AssetEnvironment>> GetAssetEnvironments()
        {

            IEnumerable<AssetEnvironment> result = new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.articles.drafts/content/css/drafts.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/plato.articles.drafts/content/js/drafts.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.articles.drafts/content/css/drafts.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/plato.articles.drafts/content/js/drafts.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.articles.drafts/content/css/drafts.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/plato.articles.drafts/content/js/drafts.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                    
                })

            };

            return Task.FromResult(result);

        }
        

    }
}
