using Microsoft.Extensions.DependencyInjection;
using Plato.Repositories.Users;
using Plato.Repositories.Settings;
using Plato.Models.Users;
using Plato.Models.Settings;

namespace Plato.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {

            services.AddScoped<ISettingRepository<Setting>, SettingRepository>();

            services.AddScoped<IUserRepository<User>, UserRepository>();
            services.AddScoped<IUserSecretRepository<UserSecret>, UserSecretRepository>();
            services.AddScoped<IUserDetailRepository<UserDetail>, UserDetailRepository>();
            services.AddScoped<IUserPhotoRepository<UserPhoto>, UserPhotoRepository>();

            return services;

        }
     

    }
}
