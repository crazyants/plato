using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Environment.Shell.Models;

namespace Plato.Environment.Shell
{
    public class ShellContextFactory : IShellContextFactory
    {
     
        private readonly ILogger _logger;

        public ShellContextFactory(ILogger<ShellContextFactory> logger)
        {
         
            _logger = logger;
        }

        ShellContext IShellContextFactory.CreateShellContext(ShellSettings settings)
        {
      
            var describedContext = CreateDescribedContext(settings, MinimumShellDescriptor());
            ShellDescriptor currentDescriptor;

            //using (var scope = describedContext.CreateServiceScope())
            //{
            //    var shellDescriptorManager = scope.ServiceProvider.GetService<IShellDescriptorManager>();
            //    currentDescriptor = shellDescriptorManager.GetShellDescriptorAsync().Result;
            //}

            //if (currentDescriptor != null)
            //{
            //    return CreateDescribedContext(settings, currentDescriptor);
            //}

            return describedContext;
        }

        //ShellContext IShellContextFactory.CreateSetupContext(ShellSettings settings)
        //{
        //    //if (_logger.IsEnabled(LogLevel.Debug))
        //    //{
        //    //    _logger.LogDebug("No shell settings available. Creating shell context for setup");
        //    //}
        //    var descriptor = new ShellDescriptor
        //    {
        //        SerialNumber = -1,
        //        Features = new[] {
        //            new ShellFeature { Name = "Orchard.Logging.Console" },
        //            new ShellFeature { Name = "Orchard.Setup" },
        //            new ShellFeature { Name = "Orchard.Recipes" }
        //        }
        //    };

        //    return CreateDescribedContext(settings, descriptor);
        //}

        public ShellContext CreateDescribedContext(ShellSettings settings, ShellDescriptor shellDescriptor)
        {
            //if (_logger.IsEnabled(LogLevel.Debug))
            //{
            //    _logger.LogDebug("Creating described context for tenant {0}", settings.Name);
            //}

            //var blueprint = _compositionStrategy.Compose(settings, shellDescriptor);
            //var provider = _shellContainerFactory.CreateContainer(settings, blueprint);

            return new ShellContext
            {
                Settings = settings
            };
        }

        private static ShellDescriptor MinimumShellDescriptor()
        {
            return new ShellDescriptor
            {
                SerialNumber = -1,
                Features = new[]
                {
                    new ShellFeature { Name = "Orchard.Logging.Console" },
                    new ShellFeature { Name = "Orchard.Hosting" },
                    new ShellFeature { Name = "Orchard.Admin" },
                    new ShellFeature { Name = "Orchard.Themes" },
                    new ShellFeature { Name = "TheAdmin" },
                    new ShellFeature { Name = "SafeMode" },
                    new ShellFeature { Name = "Orchard.Recipes" }
                }
            };
        }

    }
}
