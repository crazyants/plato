using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Modules
{
    public class ModuleOptionsConfigure : IConfigureOptions<ModuleOptions>    
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ModuleOptionsConfigure(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public void Configure(ModuleOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();
                
                var section = configuration.GetSection("Plato");
                if (section != null)
                {
                    var children = section.GetChildren();
                    foreach (var child in children)
                    {
                        if (child.Key.Contains("VirtualPathToModulesFolder"))
                            options.VirtualPathToModulesFolder = child.Value;
                    }

                }
                
            }

        }
        
    }
}
