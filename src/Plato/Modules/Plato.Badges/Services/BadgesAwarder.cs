using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Internal.Shell.Extensions;

namespace Plato.Badges.Services
{
    
    public class BadgesAwarder<TBadge> : IBadgesAwarder where TBadge : class, IBadge
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IBadgesManager<TBadge> _badgesManager;
        private readonly ILogger<BadgesAwarder<TBadge>> _logger;
        private readonly IServiceCollection _applicationServices;

        public BadgesAwarder(
            IBadgesManager<TBadge> badgesManager, 
            ILogger<BadgesAwarder<TBadge>> logger,
            IServiceCollection applicationServices,
            IServiceProvider serviceProvider)
        {
            _badgesManager = badgesManager;
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Invoke()
        {

            // Get all registered basges
            var badges = _badgesManager.GetBadges();

            // We need badges
            if (badges == null)
            {
                return;
            }
            
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Starting badge awarders.");
            }

            // Expose all registered services to the AwarderContext so the awarder can do some work
            var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new BadgeAwarderContext(clonedServices.BuildServiceProvider());
        
            // Iterate badges invoking each badge awarder delegate
            foreach (var badge in badges)
            {
                context.Badge = badge;

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Invoking awarder for badge {badge.Name}.");
                }

                badge.Awarder?.Invoke(context);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Awarder for badge {badge.Name} has been registered.");
                }

            }
            
        }

    }


}
