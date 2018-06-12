using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Repositories.Abstract;
using Plato.Internal.Models.Users;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Settings;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Repositories.Roles;

namespace Plato.Internal.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {

            // dictionary storage (used for unique key value paris - i.e. global settings
            services.AddScoped<IDictionaryRepository<DictionaryEntry>, DictionaryRepository>();
            services.AddScoped<IDictionaryFactory, DictionaryFactory>();

            // roles
            services.AddScoped<IRoleRepository<Role>, RoleRepository>();

            // users
            services.AddScoped<IUserRepository<User>, UserRepository>();
            services.AddScoped<IUserSecretRepository<UserSecret>, UserSecretRepository>();
            services.AddScoped<IUserDetailRepository<UserDetail>, UserDetailRepository>();
            services.AddScoped<IUserPhotoRepository<UserPhoto>, UserPhotoRepository>();
            services.AddScoped<IUserBannerRepository<UserBanner>, UserBannerRepository>();
            services.AddScoped<IUserRolesRepository<UserRole>, UserRolesRepository>();
            services.AddScoped<IRoleRepository<Role>, RoleRepository>();

            return services;

        }
     

    }
}
