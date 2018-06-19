using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Features;

namespace Plato.Demo.Services
{

    public class FeatureEventHandler : IFeatureEventHandler
    {

        private const string FeatureId = "Plato.Demo";

        private readonly ILogger<FeatureEventHandler> _logger;

        public FeatureEventHandler(
            ILogger<FeatureEventHandler> logger)
        {
            _logger = logger;
        }
      
        public Task InstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            //throw new Exception("This is a test exception from Plato.Demos");

            _logger.LogInformation(context.Feature.Id, $"Installing event raised within {context.Feature.Id}.");

            return Task.CompletedTask;

        }

        public Task InstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation(context.Feature.Id, $"Installed event raised within {context.Feature.Id}." );

            return Task.CompletedTask;

        }

        public Task UninstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            //throw new Exception("This is a test exception from Plato.Demos");

            _logger.LogInformation(context.Feature.Id, $"Uninstalling event raised within {context.Feature.Id}.");

            return Task.CompletedTask;

        }

        public Task UninstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation(context.Feature.Id, $"Uninstalled event raised within {context.Feature.Id}.");

            return Task.CompletedTask;
        }
    }
}
