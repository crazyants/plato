using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models;
using Plato.Internal.Repositories.Users;
using Plato.Internal.Repositories.Abstract;
using Plato.Internal.Models.Users;
using Plato.Internal.Models.Roles;
using Plato.Internal.Models.Abstract;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Repositories.Badges;
using Plato.Internal.Repositories.Metrics;
using Plato.Internal.Repositories.Reputations;
using Plato.Internal.Repositories.Roles;
using Plato.Internal.Repositories.Schema;
using Plato.Internal.Repositories.Shell;

namespace Plato.Internal.Repositories.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoRepositories(
            this IServiceCollection services)
        {

            // Shell features
            services.AddScoped<IShellFeatureRepository<ShellFeature>, ShellFeatureRepository>();
            
            // Abstract storage (used for unique key value paris - i.e. global settings
            services.AddScoped<IDictionaryRepository<DictionaryEntry>, DictionaryRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Roles
            services.AddScoped<IRoleRepository<Role>, RoleRepository>();

            // Users
            services.AddScoped<IUserRepository<User>, UserRepository>();
            services.AddScoped<IUserSecretRepository<UserSecret>, UserSecretRepository>();
            services.AddScoped<IUserDataRepository<UserData>, UserDataRepository>();
            services.AddScoped<IUserPhotoRepository<UserPhoto>, UserPhotoRepository>();
            services.AddScoped<IUserBannerRepository<UserBanner>, UserBannerRepository>();
            services.AddScoped<IUserRolesRepository<UserRole>, UserRolesRepository>();
            services.AddScoped<IRoleRepository<Role>, RoleRepository>();

            // User reputations
            services.AddScoped<IUserReputationsRepository<UserReputation>, UserReputationsRepository>();
            services.AddScoped<IAggregatedReputationMetricsRepository, AggregatedReputationMetricsRepository>();

            // User badges
            services.AddScoped<IUserBadgeRepository<UserBadge>, UserBadgeRepository>();

            // Schema 
            services.AddScoped<IConstraintRepository, ConstraintRepository>();

            // Metrics 
            services.AddScoped<IAggregatedUserMetricsRepository, AggregatedUserMetricsRepository>();


            return services;

        }
     
    }

}
