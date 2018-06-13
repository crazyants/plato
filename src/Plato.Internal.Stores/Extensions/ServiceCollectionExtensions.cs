using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Stores.Settings;
using Plato.Internal.Stores.Roles;
using Plato.Internal.Stores.Users;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Files;

namespace Plato.Internal.Stores.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddStores(
         this IServiceCollection services)
        {

            // Abstract dictionaty store (settings etc)
            services.AddScoped<IDictionaryStore, DictionaryStore>();
            
            // Ensure query is aware of current db context
            services.AddScoped<IDbQuery, DbQuery>();

            // Files
            services.AddScoped<IFileStore, FileStore>();

            // Site Settings
            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();
            
            // Roles
            services.AddScoped<IPlatoRoleStore, PlatoRoleStore>();
         
            // Users
            services.AddScoped<IPlatoUserStore<User>, PlatoUserStore>();
            services.AddScoped<IUserPhotoStore<UserPhoto>, UserPhotoStore>();
            services.AddScoped<IUserBannerStore<UserBanner>, UserBannerStore>();

            return services;


        }

    }
}
