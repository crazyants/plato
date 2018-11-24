using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Badges.Services
{

    public class BadgesAwarderManager<TBadge> : IBadgesAwarderManager where TBadge : class, IBadge
    {

        private readonly IEnumerable<IBadgesAwarderProvider<TBadge>> _providers;

        private readonly IBadgesManager<TBadge> _badgesManager;
        private readonly ILogger<BadgesAwarderManager<TBadge>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceCollection _applicationServices;
        private readonly IBackgroundTaskManager _backgroundTaskManager;

        public BadgesAwarderManager(
            IBadgesManager<TBadge> badgesManager,
            ILogger<BadgesAwarderManager<TBadge>> logger,
            IEnumerable<IBadgesAwarderProvider<TBadge>> providers,
            IServiceProvider serviceProvider, IServiceCollection applicationServices,
            IBackgroundTaskManager backgroundTaskManager)
        {
            _badgesManager = badgesManager;
            _providers = providers;
            _serviceProvider = serviceProvider;
            _applicationServices = applicationServices;
            _backgroundTaskManager = backgroundTaskManager;
            _logger = logger;
        }

        public void Award(ref IServiceProvider serviceProvider)
        {

            // Clone services to expose to notification providers
            // Some notification may need these services as they run on a background thread
            //var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);

            // Ensure we have awarders
            if (_providers == null)
            {
                return;
            }

            // Get badges
            var badges = _badgesManager.GetBadges();
            if (badges == null)
            {
                return;
            }

            var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new BadgeAwarderContext<TBadge>(clonedServices.BuildServiceProvider());
            
            //context.Badge = badge;
            foreach (var awarder in _providers)
            {
                _backgroundTaskManager.Start(async (services, args) =>
                    {

                        var result = await awarder.Award(context);
                        if (result != null)
                        {
                            if (result.Succeeded)
                            {
                                if (_logger.IsEnabled(LogLevel.Information))
                                {
                                    _logger.LogInformation($"Awarder for badge has been registered.");
                                }
                            }
                            else
                            {
                                foreach (var error in result.Errors)
                                {
                                    _logger.LogError(error.Code, error.Description);
                                }
                            }
                        }
                        
                    }, awarder.IntervalInSeconds * 1000);

            }

        }

    }

}
