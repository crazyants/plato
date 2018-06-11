﻿using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Repositories.Settings;
using Plato.Internal.Models.Users;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Settings;
using Plato.Abstractions.Settings;
using Plato.Internal.Repositories.Roles;

namespace Plato.Internal.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddRepositories(
            this IServiceCollection services)
        {

            services.AddScoped<ISettingRepository<Setting>, SettingRepository>();
            services.AddScoped<ISettingsFactory, SettingsFactory>();

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