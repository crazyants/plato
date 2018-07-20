using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Features.Abstractions;

namespace Plato.Core.Handlers
{
    public class FeatureEventHandler : BaseFeatureEventHandler
    {

    
        private readonly ILogger<BaseFeatureEventHandler> _logger;

        public FeatureEventHandler(ILogger<BaseFeatureEventHandler> logger)
        {
            _logger = logger;
        }

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task InstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override Task UninstalledAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

    }
}
