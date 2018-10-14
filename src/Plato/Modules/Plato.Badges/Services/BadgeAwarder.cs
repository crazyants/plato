using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Internal.Shell.Extensions;

namespace Plato.Badges.Services
{
    
    public class BadgeAwarder : IBadgeAwarder
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IBadgesManager<Badge> _badgesManager;
        private readonly ILogger<BadgeAwarder> _logger;
        private readonly IServiceCollection _applicationServices;

        public BadgeAwarder(
            IBadgesManager<Badge> badgesManager, 
            ILogger<BadgeAwarder> logger,
            IServiceCollection applicationServices,
            IServiceProvider serviceProvider)
        {
            _badgesManager = badgesManager;
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void StartAwarding()
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

            // Expose all registered services to the AwarderContext
            var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new AwarderContext(clonedServices.BuildServiceProvider());
        
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
