using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Features;
using Plato.Internal.Features.Abstractions;

namespace Plato.Internal.Shell
{

    public class ShellContainerFactory : IShellContainerFactory
    {
                
        private readonly IServiceCollection _applicationServices;
        private readonly IServiceProvider _serviceProvider;

        public ShellContainerFactory(
            IServiceProvider serviceProvider,    
            IServiceCollection applicationServices)
        {
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;        
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

            // TODO: Add checks to ensure type is actually enabled as a feature within our shell descriptor
            foreach (var type in blueprint.Dependencies
                .Where(t => typeof(IStartup).IsAssignableFrom(t.Key)))
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

            // Make the blueprint available to modules
            moduleServiceCollection.AddSingleton(blueprint);

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
            tenantServiceCollection.AddTransient<IShellDescriptorManager, Features.ShellDescriptorManager>();
        }


    }

}