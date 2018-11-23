using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;

namespace Plato.Badges.Services
{
    
    public class BadgesAwarderManager<TBadge> : IBadgesAwarderManager where TBadge : class, IBadge
    {

        private readonly IEnumerable<IBadgesAwarderProvider<TBadge>> _providers;
        
        private readonly IBadgesManager<TBadge> _badgesManager;
        private readonly ILogger<BadgesAwarderManager<TBadge>> _logger;
  
        public BadgesAwarderManager(
            IBadgesManager<TBadge> badgesManager, 
            ILogger<BadgesAwarderManager<TBadge>> logger,
            IEnumerable<IBadgesAwarderProvider<TBadge>> providers)
        {
            _badgesManager = badgesManager;
            _providers = providers;
            _logger = logger;
        }
        
        public void Award()
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
            
            // Invoke awarders for each badge
            foreach (var badge in badges)
            {

                var context = new BadgeAwarderContext<TBadge>(badge);

                foreach (var awarder in _providers)
                {

                    var result = awarder?.Award(context);
                    if (result != null)
                    {
                        if (result.Succeeded)
                        {
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation($"Awarder for badge {badge.Name} has been registered.");
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

                }

            }

        }

    }
    
}
