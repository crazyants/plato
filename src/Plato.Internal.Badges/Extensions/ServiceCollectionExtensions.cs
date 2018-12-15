using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;

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
