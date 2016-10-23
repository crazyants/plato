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

namespace Plato.Shell
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

     

        public IServiceProvider CreateContainer(ShellSettings settings)
        {

            IServiceCollection tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

            tenantServiceCollection.AddSingleton(settings);


            var dbContext = new DbContext(cfg =>
            {
                cfg.ConnectionString = settings.ConnectionString;
                cfg.DatabaseProvider = settings.DatabaseProvider;
                cfg.TablePrefix = settings.TablePrefix;
            });

            tenantServiceCollection.AddSingleton<IDbContextt>(dbContext);


            // add already instanciated services like DefaultOrchardHost
            var applicationServiceDescriptors = _applicationServices.Where(x => x.Lifetime == ServiceLifetime.Singleton);
            
            var shellServiceProvider = tenantServiceCollection.BuildServiceProvider();

            // Register event handlers on the event bus

            //.Select(x => x.ImplementationType)
            //.Distinct()
            //.Where(t => t != null && typeof(IEventHandler).IsAssignableFrom(t) && t.GetTypeInfo().IsClass)
            //.ToArray();

            var services = applicationServiceDescriptors
                .Select(x => x.ImplementationType)
                .Distinct()
                .Where(t => t != null && t.GetTypeInfo().IsClass)
                .ToArray();

            foreach (var service in applicationServiceDescriptors)
            {
                tenantServiceCollection.AddScoped(service.ServiceType, service.ImplementationFactory);
            }
            

            return shellServiceProvider;

        }
    }


}