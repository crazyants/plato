using System;
using System.Collections.Generic;
using System.Text;
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
        
        public async Task InstallingAsync(IShellFeature feature)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstallingAsync(this, new ShellFeatureEventArgs() {Feature = feature});
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Installing' event when activating feature: {feature.Id}");
                }
            }
        }

        public async Task InstalledAsync(IShellFeature feature)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstalledAsync(this, new ShellFeatureEventArgs() { Feature = feature });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Installed' event when activating feature: {feature.Id}");
                }
            }
        }

        public async Task UninstallingAsync(IShellFeature feature)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.InstalledAsync(this, new ShellFeatureEventArgs() { Feature = feature });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Uninstalling' event when activating feature: {feature.Id}");
                }
            }
        }

        public async Task UninstalledAsync(IShellFeature feature)
        {
            foreach (var eventProvider in _eventProviders)
            {
                try
                {
                    await eventProvider.UninstalledAsync(this, new ShellFeatureEventArgs() { Feature = feature });
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while raising the 'Uninstalled' event when activating feature: {feature.Id}");
                }
            }
        }
    }
}
