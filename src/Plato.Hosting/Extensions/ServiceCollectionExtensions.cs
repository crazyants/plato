using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Shell;
using Plato.FileSystem.AppData;
using Plato.FileSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Plato.Hosting.Extensions
{
    public static class ServiceCollectionExtensions
    {
 
        public static IServiceCollection AddPlatoHostCore(
            this IServiceCollection services)
        {
            
            services.AddSingleton<DefaultPlatoHost>();
            services.AddSingleton<IPlatoHost>(sp => sp.GetRequiredService<DefaultPlatoHost>());

            // shell / tenant context

            services.AddSingleton<IShellSettingsManager, ShellSettingsManager>();
            services.AddSingleton<IShellContextFactory, ShellContextFactory>();
            {              
                services.AddSingleton<IShellContainerFactory, ShellContainerFactory>();
            }

            services.AddSingleton<IRunningShellTable, RunningShellTable>();

            return services;
        }


    }

}
