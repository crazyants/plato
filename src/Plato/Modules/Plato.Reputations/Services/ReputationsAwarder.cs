using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Shell.Extensions;
using Plato.Reputations.Models;

namespace Plato.Reputations.Services
{

    public class ReputationsAwarder<TReputation> : IReputationsAwarder where TReputation : class, IReputation
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IReputationsManager<TReputation> _reputationsManager;
        private readonly ILogger<ReputationsAwarder<TReputation>> _logger;
        private readonly IServiceCollection _applicationServices;

        public ReputationsAwarder(
            IReputationsManager<TReputation> reputationsManager,
            ILogger<ReputationsAwarder<TReputation>> logger,
            IServiceCollection applicationServices,
            IServiceProvider serviceProvider)
        {
            _reputationsManager = reputationsManager;
            _applicationServices = applicationServices;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Invoke()
        {

            // Get all registered reputations
            var reputations = _reputationsManager.GetReputations();

            // We need reputations
            if (reputations == null)
            {
                return;
            }

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation($"Starting reputations awarders.");
            }

            // Expose all registered services to the AwarderContext so the awarder can do some work
            var clonedServices = _serviceProvider.CreateChildContainer(_applicationServices);
            var context = new ReputationAwarderContext(clonedServices.BuildServiceProvider());

            // Iterate reputations invoking each reputation awarder delegate
            foreach (var reputation in reputations)
            {
                context.Reputation = reputation;

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Invoking awarder for reputation {reputation.Name}.");
                }

                reputation.Awarder?.Invoke(context);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"Awarder for reputation {reputation.Name} has been registered.");
                }

            }
            
        }

    }

}
