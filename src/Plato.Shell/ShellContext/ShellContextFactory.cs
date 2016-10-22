using Microsoft.Extensions.Logging;
using Plato.Shell.Models;

namespace Plato.Shell
{
    public class ShellContextFactory : IShellContextFactory
    {
           
        private readonly ILogger _logger;

        public ShellContextFactory(
            ILogger<ShellContextFactory> logger)
        {          
            _logger = logger;
        }

        ShellContext IShellContextFactory.CreateShellContext(ShellSettings settings)
        {

            return CreateDescribedContext(settings, MinimumShellDescriptor());
            
        }

        public ShellContext CreateDescribedContext(ShellSettings settings, ShellDescriptor shellDescriptor)
        {

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Creating described context for tenant {0}", settings.Name);
            }

            //var blueprint = _compositionStrategy.Compose(settings, shellDescriptor);
            //var provider = _shellContainerFactory.CreateContainer(settings, blueprint);

            return new ShellContext
            {
                Settings = settings                
            };

        }

        ShellContext IShellContextFactory.CreateSetupContext(ShellSettings settings)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("No shell settings available. Creating shell context for setup");
            }
            var descriptor = new ShellDescriptor
            {
                SerialNumber = -1,
                Modules = new[] {
                    new ShellModule { Name = "Orchard.Logging.Console" },
                    new ShellModule { Name = "Orchard.Setup" },
                    new ShellModule { Name = "Orchard.Recipes" }
                }
            };

            return CreateDescribedContext(settings, descriptor);
        }


        private static ShellDescriptor MinimumShellDescriptor()
        {
            return new ShellDescriptor
            {
                SerialNumber = -1,
                Modules = new[]
                {
                    new ShellModule { Name = "Orchard.Logging.Console" },
                    new ShellModule { Name = "Orchard.Hosting" },
                    new ShellModule { Name = "Orchard.Admin" },
                    new ShellModule { Name = "Orchard.Themes" },
                    new ShellModule { Name = "TheAdmin" },
                    new ShellModule { Name = "SafeMode" },
                    new ShellModule { Name = "Orchard.Recipes" }
                }
            };
        }

    }
}
