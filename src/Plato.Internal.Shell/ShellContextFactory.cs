using Microsoft.Extensions.Logging;
using Plato.Internal.Shell.Abstractions.Models;

namespace Plato.Internal.Shell
{
    public class ShellContextFactory : IShellContextFactory
    {
        
        private readonly ICompositionStrategy _compositionStrategy;
        private readonly IShellContainerFactory _shellContainerFactory;
        private readonly ILogger _logger;

        public ShellContextFactory(
            IShellContainerFactory shellContainerFactory,
            ICompositionStrategy compositionStrategy,
            ILogger<ShellContextFactory> logger)
        {
            _shellContainerFactory = shellContainerFactory;
            _compositionStrategy = compositionStrategy;
            _logger = logger;
        }

        ShellContext IShellContextFactory.CreateShellContext(ShellSettings settings)
        {
            return CreateDescribedContext(settings, MinimumShellDescriptor());
        }

        public ShellContext CreateDescribedContext(ShellSettings settings, ShellDescriptor descriptor)
        {

            if (_logger.IsEnabled(LogLevel.Debug))            
                _logger.LogDebug("Creating described context for tenant {0}", settings.Name);
            
            var blueprint =  _compositionStrategy.ComposeAsync(settings, descriptor).Result;
            var serviceProvider = _shellContainerFactory.CreateContainer(settings, blueprint);
                                    
            return new ShellContext
            {
                Settings = settings,
                Blueprint = blueprint,
                ServiceProvider = serviceProvider              
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
                Modules = new[] {
                    new ShellFeature { Name = "Plato.SetUp" }
                }
            };

            return CreateDescribedContext(settings, descriptor);
        }
        
        private static ShellDescriptor MinimumShellDescriptor()
        {
            return new ShellDescriptor
            {
                Modules = new[]
                {
                    new ShellFeature { Name = "Plato.Core" },
                    new ShellFeature { Name = "Plato.Admin" },
                    new ShellFeature { Name = "Plato.Users" },
                    new ShellFeature { Name = "Plato.Roles" },
                    new ShellFeature { Name = "Plato.Settings" },
                    new ShellFeature { Name = "Plato.Modules" }
                }
            };
        }

    }
}
