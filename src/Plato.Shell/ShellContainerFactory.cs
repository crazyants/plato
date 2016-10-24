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

            try
            {

                IServiceCollection tenantServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

                tenantServiceCollection.AddSingleton(settings);

                // modules

                //IServiceCollection moduleServiceCollection = _serviceProvider.CreateChildContainer(_applicationServices);

                ////foreach (var dependency in blueprint.Dependencies.Where(t => typeof(IStartup).IsAssignableFrom(t.Type)))
                ////{
                ////    moduleServiceCollection.AddSingleton(typeof(IStartup), dependency.Type);
                ////    tenantServiceCollection.AddSingleton(typeof(IStartup), dependency.Type);
                ////}

                //// Make shell settings available to the modules
                //moduleServiceCollection.AddSingleton(settings);

                //var moduleServiceProvider = moduleServiceCollection.BuildServiceProvider();

                //// Let any module add custom service descriptors to the tenant
                //foreach (var service in moduleServiceProvider.GetServices<IStartup>())
                //{
                //    service.ConfigureServices(tenantServiceCollection);
                //}

                //(moduleServiceProvider as IDisposable).Dispose();
                
                // configure data access

                var dbContext = new DbContext(cfg =>
                {
                    cfg.ConnectionString = settings.ConnectionString;
                    cfg.DatabaseProvider = settings.DatabaseProvider;
                    cfg.TablePrefix = settings.TablePrefix;
                });


                var dbContextOptions = new DbContextOptions()
                {
                    ConnectionString = settings.ConnectionString,
                    DatabaseProvider = settings.DatabaseProvider,
                    TablePrefix = settings.TablePrefix

                };

                //tenantServiceCollection.AddSingleton<IConfigureOptions<DbContextOptions>>(spdbContextOptions);
                
                tenantServiceCollection.AddSingleton<IDbContext>(dbContext);
                tenantServiceCollection.AddScoped<IDbContext>(sp => dbContext);


                // add already instanciated services like DefaultPlatoHost
                //var applicationServiceDescriptors = _applicationServices.Where(x => x.Lifetime == ServiceLifetime.Singleton);

                //var services = applicationServiceDescriptors
                //    .Select(x => x.ImplementationType)
                //    .Distinct()
                //    .Where(t => t != null && t.GetTypeInfo().IsClass)
                //    .ToArray();

                //foreach (var service in applicationServiceDescriptors)
                //{
                //    tenantServiceCollection.AddScoped(service.ServiceType, service.ImplementationFactory);
                //}

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