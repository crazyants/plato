using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Discuss.Handlers
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

    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        public string Version { get; } = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;
        

        public FeatureEventHandler(ISchemaBuilder schemaBuilder)
        {
            _schemaBuilder = schemaBuilder;
        }
        
        #region "Implementation"

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

        #endregion

        #region "Private Methods"

        #endregion
        
    }
}
