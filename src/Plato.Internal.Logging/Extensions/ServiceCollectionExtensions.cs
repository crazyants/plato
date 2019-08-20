using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Logging.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoLogging(
            this IServiceCollection services)
        {

            services.AddSingleton<ILoggerProvider, FileLoggerProvider>();

            return services;

        }


    }

}
