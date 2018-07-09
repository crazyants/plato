using System.Collections.Generic;

namespace Plato.Internal.Assets.Abstractions
{
    public class DefaultAssets
    {

        public static IEnumerable<AssetEnvironment> GetDefaultResources()
        {

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(Environment.Development, new List<Asset>()
                {
                    /* Css */

                    new Asset()
                    {
                        Url = "/css/vendors/bootstrap.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/vendors/font-awesome.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    //new Resource()
                    //{
                    //    Url = "/css/app/markdown.css",
                    //    Type = ResourceType.Css,
                    //    Section = ResourceSection.Header
                    //},
                    new Asset()
                    {
                        Url = "/css/app/plato.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },

                    /* JavaScript */

                    new Asset()
                    {
                        Url = "/js/vendors/jquery.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/vue.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    //new Resource()
                    //{
                    //    Url = "/js/app/markdown.js",
                    //    Type = ResourceType.JavaScript,
                    //    Section = ResourceSection.Footer
                    //},
                    new Asset()
                    {
                        Url = "/js/app/plato.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(Environment.Staging, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = "~/css/app.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    /* JavaScript */
                    new Asset()
                    {
                        Url = "~/js/app.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                }),

                // Production
                new AssetEnvironment(Environment.Production, new List<Asset>()
                {
                    /* Css */
                    new Asset()
                    {
                        Url = "~/css/app.min.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    /* JavaScript */
                    new Asset()
                    {
                        Url = "~/js/app.min.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                })

            };

        }

    }

}
