using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Badges.Services;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Shell.Extensions;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Badges.Services
{

    public interface IBadgeAwarderInvoker
    {
        void Invoke();
    }

    public class BadgeAwarderInvoker : IBadgeAwarderInvoker
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IBadgesManager<Badge> _badgesManager;
        private readonly IBackgroundTaskManager _backgroundTaskManager;
        private readonly IDbContext _dbContext;
        private readonly ILogger<BadgeAwarderInvoker> _logger;
        private readonly IServiceCollection _applicationServices;

        public BadgeAwarderInvoker(
            IBackgroundTaskManager backgroundTaskManager,
            IBadgesManager<Badge> badgesManager, 
            ILogger<BadgeAwarderInvoker> logger,
            IDbContext dbContext,
            IServiceCollection applicationServices,
            IServiceProvider serviceProvider)
        {
            _backgroundTaskManager = backgroundTaskManager;
            _badgesManager = badgesManager;
            _dbContext = dbContext;
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

            var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new AwarderContext(clonedServices.BuildServiceProvider());
        
            // Iterate badges invoking each badge awarder
            foreach (var badge in badges)
            {
                context.Badge = badge;
                badge.Awarder?.Invoke(context);
            }
            
        }

    }


}
