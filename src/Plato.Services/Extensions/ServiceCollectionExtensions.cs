using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Settings;
using Plato.Services.Settings;

namespace Plato.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddServices(
         this IServiceCollection services)
        {

            services.AddScoped<ISiteService, SiteService>();
         
            return services;

        }

    }
}
