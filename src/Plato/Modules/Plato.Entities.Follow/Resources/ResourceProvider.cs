using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Resources.Abstractions;

namespace Plato.Entities.Follow.Resources
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
                        Url = "/plato.entities.follow/content/js/follow.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Staging
                new ResourceEnvironment(Environment.Staging, new List<Resource>()
                {
                    new Resource()
                    {
                        Url = "/plato.entities.follow/content/js/follow.min.js",
                        Type = ResourceType.JavaScript,
                        Section = ResourceSection.Footer
                    }
                }),

                // Production
                new ResourceEnvironment(Environment.Production, new List<Resource>()
                {
                    new Resource()
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
