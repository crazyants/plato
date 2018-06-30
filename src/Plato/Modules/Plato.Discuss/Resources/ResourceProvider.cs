using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Resources.Abstractions;

namespace Plato.Discuss.Resources
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
                        Url = "/plato.discuss/content/css/discuss.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Resource()
                    {
                        Url = "/plato.discuss/content/js/discuss.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Staging
                new ResourceEnvironment(Environment.Staging, new List<Resource>()
                {
                    new Resource()
                    {
                        Url = "/plato.discuss/content/css/discuss.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Resource()
                    {
                        Url = "/plato.discuss/content/js/discuss.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Production
                new ResourceEnvironment(Environment.Production, new List<Resource>()
                {
                    new Resource()
                    {
                        Url = "/plato.discuss/content/css/discuss.css",
                        Type = ResourceType.Css,
                        Section = ResourceSection.Header
                    },
                    new Resource()
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
