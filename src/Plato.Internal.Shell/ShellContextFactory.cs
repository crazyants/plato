using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

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

        ShellContext IShellContextFactory.CreateShellContext(IShellSettings settings)
        {

            // Build minimal shell descriptor
            var describedContext = CreateDescribedContext(settings, MinimumShellDescriptor());
            
            // Do we have a descriptor within the database
            IShellDescriptor currentDescriptor = null;
            using (var scope = describedContext.CreateServiceScope())
            {
                var shellDescriptorStore = scope.ServiceProvider.GetService<IShellDescriptorStore>();
                var descriptor = shellDescriptorStore.GetAsync().Result;
                if (descriptor != null)
                {
                    currentDescriptor = descriptor;
                }
            }

            if (currentDescriptor != null)
            {
                return CreateDescribedContext(settings, currentDescriptor);
            }

            return describedContext;

        }

        public ShellContext CreateDescribedContext(IShellSettings settings, IShellDescriptor descriptor)
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

        ShellContext IShellContextFactory.CreateSetupContext(IShellSettings settings)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("No shell settings available. Creating shell context for setup");
            }
            var descriptor = new ShellDescriptor
            {
                Modules = new[] {
                    new ShellModule { Id = "Plato.SetUp" }
                }
            };

            return CreateDescribedContext(settings, descriptor);
        }
        
        private ShellDescriptor MinimumShellDescriptor()
        {
            return new ShellDescriptor
            {
                Modules = new[]
                {
                    new ShellModule { Id = "Plato.Core" },
                    new ShellModule { Id = "Plato.Admin" },
                    new ShellModule { Id = "Plato.Users" },
                    new ShellModule { Id = "Plato.Roles" },
                    new ShellModule { Id = "Plato.Settings" },
                    new ShellModule { Id = "Plato.Features" }
                }
            };
        }
        
    }

}
