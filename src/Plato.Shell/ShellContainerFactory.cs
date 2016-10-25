using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Shell.Models;
using Plato.Shell.Extensions;
using Plato.Data;
using System.Reflection;
using Plato.Abstractions;
using Microsoft.Extensions.Options;
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

            try
            {

                // clone services
                IServiceCollection tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

                // add tenant settings
                tenantServiceCollection.AddSingleton(settings);
                           
                // add tenant specific DbContext
                tenantServiceCollection.AddScoped<IDbContext>(sp => new DbContext(cfg =>
                {
                    cfg.ConnectionString = settings.ConnectionString;
                    cfg.DatabaseProvider = settings.DatabaseProvider;
                    cfg.TablePrefix = settings.TablePrefix;
                }));


                // add service descriptors from modules to the tenant

                IServiceCollection moduleServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

                var assemblies = _moduleManager.AllAvailableAssemblies;

                var types = new List<Type>();
                foreach (Assembly assmebly in assemblies)
                {
                    types.AddRange(assmebly.GetTypes());
                }

                foreach (var type in types.Where(t => typeof(IStartup).IsAssignableFrom(t)))
                {
                    moduleServiceCollection.AddSingleton(typeof(IStartup), type);
                    tenantServiceCollection.AddSingleton(typeof(IStartup), type);
                }

                // Make shell settings available to the modules
                moduleServiceCollection.AddSingleton(settings);

                var moduleServiceProvider = moduleServiceCollection.BuildServiceProvider();                            
                foreach (var service in moduleServiceProvider.GetServices<IStartup>())
                {
                    service.ConfigureServices(tenantServiceCollection);
                }

                (moduleServiceProvider as IDisposable).Dispose();



                // return



                var shellServiceProvider = tenantServiceCollection.BuildServiceProvider();
                return shellServiceProvider;

            }
            catch (Exception e)
            {

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Error creating container for tenant {0} - {1}", settings.Name, e.Message);
                }

            }

            return null;
            
        }

    }

}