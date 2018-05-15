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
using Plato.Models.Users;
using Plato.Stores.Files;

namespace Plato.Stores.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddStores(
         this IServiceCollection services)
        {

            // files

            services.AddScoped<IFileStore, FileStore>();

            // settings

            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();
            
            // roles

            services.AddScoped<IPlatoRoleStore, PlatoRoleStore>();

            // users

            services.AddScoped<IPlatoUserStore<User>, PlatoUserStore>();
            services.AddScoped<IUserPhotoStore<UserPhoto>, UserPhotoStore>();
            services.AddScoped<IUserBannerStore<UserBanner>, UserBannerStore>();

            return services;


        }

    }
}
