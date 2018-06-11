using Microsoft.Extensions.Logging;
using Plato.Internal.Shell.Models;
using Plato.Internal.Data;
using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

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


            var blueprint = _compositionStrategy.ComposeAsync(settings, descriptor);
            var serviceProvider = _shellContainerFactory.CreateContainer(settings);
                                    
            return new ShellContext
            {
                Settings = settings,
                Descriptor = descriptor,
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
                    new ShellModule { Name = "Plato.Logging.Console" },
                    new ShellModule { Name = "Plato.Setup" },
                    new ShellModule { Name = "Plato.Recipes" }
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
                    new ShellModule { Name = "Plato.Logging.Console" },
                    new ShellModule { Name = "Plato.Hosting" },
                    new ShellModule { Name = "Plato.Admin" },
                    new ShellModule { Name = "Plato.Themes" },
                    new ShellModule { Name = "TheAdmin" },
                    new ShellModule { Name = "SafeMode" },
                    new ShellModule { Name = "Plato.Recipes" }
                }
            };
        }

    }
}
