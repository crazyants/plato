using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Features;

namespace Plato.Internal.Features
{
    
    public class FeatureEventManager : IFeatureEventManager
    {

        private readonly IEnumerable<IFeatureEventHandler> _eventProviders;
        private readonly ILogger<FeatureEventManager> _logger;
        
        public FeatureEventManager(
            IEnumerable<IFeatureEventHandler> eventProviders,
            ILogger<FeatureEventManager> logger)
        {
            _eventProviders = eventProviders;
            _logger = logger;
        }
        
        public async Task<IFeatureEventContext> InstallingAsync(IFeatureEventContext context)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstallingAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Installing' event within feature: {context.Feature.Id}");
                    context.Errors.Add(context.Feature.Id, e.Message);
                }
            }

            return context;
        }

        public async Task<IFeatureEventContext> InstalledAsync(IFeatureEventContext context)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstalledAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Installed' event within feature: {context.Feature.Id}");
                    context.Errors.Add(context.Feature.Id, e.Message);
                }
            }

            return context;

        }

        public async Task<IFeatureEventContext> UninstallingAsync(IFeatureEventContext context)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstalledAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Uninstalling' event within feature: {context.Feature.Id}");
                    context.Errors.Add(context.Feature.Id, e.Message);
                }
            }

            return context;

        }

        public async Task<IFeatureEventContext> UninstalledAsync(IFeatureEventContext context)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.UninstalledAsync(context);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Uninstalled' event within feature: {context.Feature.Id}");
                    context.Errors.Add(context.Feature.Id, e.Message);
                }
            }

            return context;

        }
    }
}
