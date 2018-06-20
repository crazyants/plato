using System;
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

        public Task InstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }

            return Task.CompletedTask;

        }

        public Task InstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }

            return Task.CompletedTask;

        }

        public Task UninstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }

            return Task.CompletedTask;

        }

        public Task UninstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            try
            {


            }
            catch (Exception e)
            {
                context.Errors.Add(context.Feature.Id, e.Message);
            }


            return Task.CompletedTask;
        }
    }
}
