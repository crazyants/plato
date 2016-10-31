using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Abstractions.Settings;
using Plato.Abstractions.Stores;
using Plato.Stores.Settings;
using Plato.Stores.Roles;
using Plato.Stores.Users;

namespace Plato.Stores.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddStores(
         this IServiceCollection services)
        {

            // settings

            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();
            
            // roles

            services.AddScoped<IRoleStore, RoleStore>();
            services.AddScoped<IPlatoUserStore, PlatoUserStore>();

            return services;


        }

    }
}
