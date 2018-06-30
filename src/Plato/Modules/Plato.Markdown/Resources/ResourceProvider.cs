using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Resources.Abstractions;

namespace Plato.Markdown.Resources
{
    public class ResourceProvider : IResourceProvider
    {

        public Task<IEnumerable<ResourceEnvironment>> GetResourceGroups()
        {

            IEnumerable<ResourceEnvironment> result = new List<ResourceEnvironment>
            {

                // Development
                new ResourceEnvironment(Environment.Development, new List<Resource>()
                {
                    new Resource()
                    {
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
                    {
                        Url = $"/plato.markdown/content/css/markdown.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                }),

                // Staging
                new ResourceEnvironment(Environment.Staging, new List<Resource>()
                {
                    /* Css */
                    new Resource()
                    {
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
                    {
                        Url = $"/plato.markdown/content/css/markdown.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    }
                }),

                // Production
                new ResourceEnvironment(Environment.Production, new List<Resource>()
                {
                    /* Css */
                    new Resource()
                    {
                        Url = $"/plato.markdown/content/js/markdown.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    },
                    new Resource()
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
