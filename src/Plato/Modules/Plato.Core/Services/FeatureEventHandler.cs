using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Features;

namespace Plato.Core.Services
{
    public class FeatureEventHandler : IFeatureEventHandler
    {

        private const string FeatureId = "Plato.Core";

        private readonly ILogger<FeatureEventHandler> _logger;

        public FeatureEventHandler(ILogger<FeatureEventHandler> logger)
        {
            _logger = logger;
        }
      
        public Task InstallingAsync(object sender, ShellFeatureEventArgs args)
        {
            if (!String.Equals(args.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }
            
            _logger.LogInformation(args.Feature.Id, $"Installing event raised within {args.Feature.Id}.");

            return Task.CompletedTask;

        }

        public Task InstalledAsync(object sender, ShellFeatureEventArgs args)
        {
            if (!String.Equals(args.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation(args.Feature.Id, $"Installed event raised within {args.Feature.Id}." );

            return Task.CompletedTask;

        }

        public Task UninstallingAsync(object sender, ShellFeatureEventArgs args)
        {
            if (!String.Equals(args.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation(args.Feature.Id, $"Uninstalling event raised within {args.Feature.Id}.");

            return Task.CompletedTask;

        }

        public Task UninstalledAsync(object sender, ShellFeatureEventArgs args)
        {
            if (!String.Equals(args.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            _logger.LogInformation(args.Feature.Id, $"Uninstalled event raised within {args.Feature.Id}.");

            return Task.CompletedTask;
        }
    }
}
