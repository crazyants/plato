using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Shell.Models;
using Plato.Shell.Extensions;
using Plato.Data;
using Plato.Abstractions;
using Plato.Modules.Abstractions;

namespace Plato.Shell
{
    public class ShellContainerFactory : IShellContainerFactory
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceCollection _applicationServices;
        private readonly IModuleManager _moduleManager;

        public ShellContainerFactory(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            ILogger<ShellContainerFactory> logger,
            IServiceCollection applicationServices,
            IModuleManager moduleManager)
        {
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
            _logger = logger;
            _moduleManager = moduleManager;
        }

        public IServiceProvider CreateContainer(ShellSettings settings)
        {
            
            // clone services
            // ---------------

            var tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

            // add tenant settings
            // ---------------
            
            tenantServiceCollection.AddSingleton(settings);
            
            // add tenant specific DbContext
            tenantServiceCollection.AddScoped<IDbContext>(sp => new DbContext(cfg =>
            {
                cfg.ConnectionString = settings.ConnectionString;
                cfg.DatabaseProvider = settings.DatabaseProvider;
                cfg.TablePrefix = settings.TablePrefix;
            }));
      
       
            
            // add service descriptors from modules to the tenant
          
            var types = new List<Type>();
            foreach (var assmebly in _moduleManager.AllAvailableAssemblies)
                types.AddRange(assmebly.GetTypes());

            var moduleServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);
            foreach (var type in types.Where(t => typeof(IStartup).IsAssignableFrom(t)))
            {
                moduleServiceCollection.AddSingleton(typeof(IStartup), type);
                tenantServiceCollection.AddSingleton(typeof(IStartup), type);
            }

            // Add a default configuration if none has been provided

            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            moduleServiceCollection.TryAddSingleton(configuration);
            tenantServiceCollection.TryAddSingleton(configuration);

            // Make shell settings available to the modules
            moduleServiceCollection.AddSingleton(settings);

            // Configure StartUps

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

    }

}