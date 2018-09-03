using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Shell.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Features;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Internal.Shell
{
    public class ShellContainerFactory : IShellContainerFactory
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceCollection _applicationServices;
     
        public ShellContainerFactory(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            ILogger<ShellContainerFactory> logger,
            IServiceCollection applicationServices)
        {
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
            _logger = logger;
       
        }

        public IServiceProvider CreateContainer(IShellSettings settings, ShellBlueprint blueprint)
        {

            // Clone services
            var tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

            // Add tenant specific settings
            tenantServiceCollection.AddSingleton(settings);
            tenantServiceCollection.AddSingleton(blueprint.Descriptor);
            tenantServiceCollection.AddSingleton(blueprint);

            // Add tenant specific data context options
            tenantServiceCollection.Configure<DbContextOptions>(options =>
            {
                options.ConnectionString = settings.ConnectionString;
                options.DatabaseProvider = settings.DatabaseProvider;
                options.TablePrefix = settings.TablePrefix;
            });

            // Add core tennet services
            AddCoreServices(tenantServiceCollection);
            
            // Add StartUps from modules defined in blueprint descriptor as services
            var moduleServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);
            foreach (var type in blueprint.Dependencies.Where(t => typeof(IStartup).IsAssignableFrom(t.Key)))
            {
                moduleServiceCollection.AddSingleton(typeof(IStartup), type.Key);
                tenantServiceCollection.AddSingleton(typeof(IStartup), type.Key);
            }

            // Add a default configuration if none has been provided
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            moduleServiceCollection.TryAddSingleton(configuration);
            tenantServiceCollection.TryAddSingleton(configuration);

            // Make shell settings available to the modules
            moduleServiceCollection.AddSingleton(settings);

            // Configure module StartUps
            var moduleServiceProvider = moduleServiceCollection.BuildServiceProvider();
            var startups = moduleServiceProvider.GetServices<IStartup>();
            foreach (var startup in startups)
            {
                startup.ConfigureServices(tenantServiceCollection);
            }

            (moduleServiceProvider as IDisposable).Dispose();

            // return

            var shellServiceProvider = tenantServiceCollection.BuildServiceProvider();
            return shellServiceProvider;

        }

        private void AddCoreServices(IServiceCollection tenantServiceCollection)
        {
            tenantServiceCollection.AddTransient<IShellFeatureManager, ShellFeatureManager>();
            tenantServiceCollection.AddTransient<IShellDescriptorManager, ShellDescriptorManager>();

            tenantServiceCollection.AddTransient<IContextFacade, ContextFacade>();


        }


    }

}