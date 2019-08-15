using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Features;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Stores.Settings;
using Plato.Internal.Stores.Roles;
using Plato.Internal.Stores.Users;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstract;
using Plato.Internal.Stores.Abstractions;
using Plato.Internal.Stores.Abstractions.Badges;
using Plato.Internal.Stores.Abstractions.Files;
using Plato.Internal.Stores.Abstractions.Reputations;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Abstractions.Schema;
using Plato.Internal.Stores.Abstractions.Settings;
using Plato.Internal.Stores.Abstractions.Shell;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Badges;
using Plato.Internal.Stores.Files;
using Plato.Internal.Stores.Reputations;
using Plato.Internal.Stores.Schema;
using Plato.Internal.Stores.Shell;

namespace Plato.Internal.Stores.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoStores(
         this IServiceCollection services)
        {

            // Abstract stores 
            services.AddScoped<IDictionaryStore, DictionaryStore>();
            services.AddScoped<IDocumentStore, DocumentStore>();

            // Ensure query is aware of current db context
            services.AddScoped<IDbQueryConfiguration, DbQueryConfiguration>();
            
            // Files
            services.AddScoped<IFileStore, FileStore>();

            // Shell features
            services.AddScoped<IShellDescriptorStore, ShellDescriptorStore>();
            services.AddScoped<IShellFeatureStore<ShellFeature>, ShellFeatureStore>();

            // Site Settings
            services.AddScoped<ISiteSettingsStore, SiteSettingsStore>();
            
            // Roles
            services.AddScoped<IPlatoRoleStore, PlatoRoleStore>();
            services.AddScoped<IPlatoUserRoleStore<UserRole>, PlatoUserRolesStore>();

            // Users
            services.AddTransient<IPlatoUserStore<User>, PlatoUserStore>();
            services.AddScoped<IUserPhotoStore<UserPhoto>, UserPhotoStore>();
            services.AddScoped<IUserDataItemStore<UserData>, UserDataItemStore>();
            services.AddScoped<IUserDataStore<UserData>, UserDataStore>();

            // Decorators
            services.AddScoped<IUserDataDecorator, UserDataDecorator>();
            services.AddScoped<IUserRoleDecorator, UserRoleDecorator>();

            // User Reputation
            services.AddScoped<IUserReputationsStore<UserReputation>, UserReputationsStore>();

            // User badges
            services.AddScoped<IUserBadgeStore<UserBadge>, UserBadgeStore>();
            
            // Schema
            services.AddScoped<IConstraintStore, ConstraintStore>();
            
   
            return services;
            
        }

    }

}
