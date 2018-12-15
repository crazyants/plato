using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Internal.Badges.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoBadges(
            this IServiceCollection services)
        {

            services.AddScoped<IBadgesManager<Badge>, BadgesManager<Badge>>();

            return services;

        }


    }
}
