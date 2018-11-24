using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Badges.Services
{

    public class BadgesAwarderManager<TBadge> : IBadgesAwarderManager where TBadge : class, IBadge
    {

        private readonly IEnumerable<IBadgesAwarderProvider<TBadge>> _providers;

        private readonly IBadgesManager<TBadge> _badgesManager;
        private readonly ILogger<BadgesAwarderManager<TBadge>> _logger;
        private readonly IBackgroundTaskManager _backgroundTaskManager;

        public BadgesAwarderManager(
            IBadgesManager<TBadge> badgesManager,
            ILogger<BadgesAwarderManager<TBadge>> logger,
            IEnumerable<IBadgesAwarderProvider<TBadge>> providers,
            IBackgroundTaskManager backgroundTaskManager)
        {
            _badgesManager = badgesManager;
            _providers = providers;
            _backgroundTaskManager = backgroundTaskManager;
            _logger = logger;
        }

        public void Award(ref IServiceProvider serviceProvider)
        {

       
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

            //var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new BadgeAwarderContext<TBadge>();

            //context.Badge = badge;
            var results = new List<ICommandResult<TBadge>>();
            foreach (var provider in _providers)
            {

                // All providers need to know which badge they are awarding for
                if (provider.Badge == null)
                {
                    throw new Exception($"You must ensure the badge poperty is specififed for the awarder provider of type {provider.GetType()} ");
                }

                _backgroundTaskManager.Start(async (sender, args) =>
                    {

                        try
                        {
                            var result = await provider.AwardAsync(context);
                            if (result != null)
                            {
                                results.Add(result);
                                if (_logger.IsEnabled(LogLevel.Information))
                                {
                                    _logger.LogInformation($"Awarder for badge {provider.Badge.Name} registered successfully");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (_logger.IsEnabled(LogLevel.Critical))
                            {
                                _logger.LogError(
                                    $"An error occurred whilst invoking the AwardAsync method within the awarder provider of type {provider.GetType()} for badge '{provider.Badge.Name}'. Error Message: {e.Message}");
                            }
                        }

                    }, provider.IntervalInSeconds * 1000);

            }

        }

    }

}
