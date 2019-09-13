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
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    /* Css */

                    new Asset()
                    {
                        Url = "/css/vendors/bootstrap.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/vendors/font-awesome.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/app/plato.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
              
                    /* JavaScript */

                    new Asset()
                    {
                        Url = "/js/vendors/jquery.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap-notify.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/app/plato.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/app/plato-ui.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                       /* Css */

                    new Asset()
                    {
                        Url = "/css/vendors/bootstrap.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/vendors/font-awesome.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/app/plato.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
              
                    /* JavaScript */

                    new Asset()
                    {
                        Url = "/js/vendors/jquery.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap-notify.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/app/plato.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/app/plato-ui.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                     /* Css */

                    new Asset()
                    {
                        Url = "/css/vendors/bootstrap.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/vendors/font-awesome.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
                    new Asset()
                    {
                        Url = "/css/app/plato.css",
                        Type = AssetType.IncludeCss,
                        Section = AssetSection.Header
                    },
              
                    /* JavaScript */

                    new Asset()
                    {
                        Url = "/js/vendors/jquery.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/vendors/bootstrap-notify.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/app/plato.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    },
                    new Asset()
                    {
                        Url = "/js/app/plato-ui.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                })

            };

        }

    }

}
