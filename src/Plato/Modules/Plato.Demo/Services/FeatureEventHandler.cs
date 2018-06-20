using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plato.Internal.Features;

namespace Plato.Demo.Services
{

    // Feature event handlers are executed in a temporary shell context 
    // This is necessary as the feature may not be enabled and as 
    // such the event handlers for the feature won't be registered with DI
    // For example we can't invoke the Installing or Installed events within
    // the main context as the feature is currently disabled within this context
    // so the IFeatureEventHandler provider for the feature has not been registered within DI.
    // ShellFeatureManager instead creates a temporary context consisting of a shell descriptor
    // with the features we want to enable or disable. The necessary IFeatureEventHandler can
    // then be registered within DI for the features we are enabling or disabling and the events can be invoked.

    public class FeatureEventHandler : IFeatureEventHandler
    {

        private const string FeatureId = "Plato.Demo";
        
        public Task InstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }
            
            try
            {

                throw new Exception("This is a test exception from Plato.Demo XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");

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

            var logger = context.ServiceProvider.GetRequiredService<ILogger<FeatureEventHandler>>();
            logger.LogInformation(context.Feature.Id, $"Installed event raised within {context.Feature.Id}." );

            return Task.CompletedTask;

        }

        public Task UninstallingAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            //throw new Exception("This is a test exception from Plato.Demos");

            var logger = context.ServiceProvider.GetRequiredService<ILogger<FeatureEventHandler>>();
            logger.LogInformation(context.Feature.Id, $"Uninstalling event raised within {context.Feature.Id}.");

            return Task.CompletedTask;

        }

        public Task UninstalledAsync(IFeatureEventContext context)
        {
            if (!String.Equals(context.Feature.Id, FeatureId, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.CompletedTask;
            }

            var logger = context.ServiceProvider.GetRequiredService<ILogger<FeatureEventHandler>>();
            logger.LogInformation(context.Feature.Id, $"Uninstalled event raised within {context.Feature.Id}.");

            return Task.CompletedTask;
        }
    }
}
