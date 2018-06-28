using System.Collections.Generic;

namespace Plato.Internal.Resources.Abstractions
{
    public class DefaultResources
    {

        public static IEnumerable<ResourceGroup> GetDefaultResources()
        {

            return new List<ResourceGroup>
            {

                // Development
                new ResourceGroup(Environment.Development, new List<Resource>()
                {
                    /* Css */

                    new Resource()
                    {
                        Url = "/css/vendors/bootstrap.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Resource()
                    {
                        Url = "/css/vendors/font-awesome.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Resource()
                    {
                        Url = "/css/app/markdown.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Resource()
                    {
                        Url = "/css/app/plato.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },

                    /* JavaScript */

                    new Resource()
                    {
                        Url = "/js/vendors/jquery.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
                    {
                        Url = "/js/vendors/bootstrap.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
                    {
                        Url = "/js/vendors/vue.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
                    {
                        Url = "/js/app/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
                    {
                        Url = "/js/app/plato.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Staging
                new ResourceGroup(Environment.Staging, new List<Resource>()
                {
                    /* Css */
                    new Resource()
                    {
                        Url = "~/css/app.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    /* JavaScript */
                    new Resource()
                    {
                        Url = "~/js/app.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                }),

                // Production
                new ResourceGroup(Environment.Production, new List<Resource>()
                {
                    /* Css */
                    new Resource()
                    {
                        Url = "~/css/app.min.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    /* JavaScript */
                    new Resource()
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
