using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Search.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        public string Version { get; } = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;


        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            IShellFeatureStore<ShellFeature> shellFeatureStore)
        {
            _schemaBuilder = schemaBuilder;
            _shellFeatureStore = shellFeatureStore;
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;

        }

        public override async Task InstalledAsync(IFeatureEventContext context)
        {

            // Add default feature settings
            var features = await _shellFeatureStore.SelectFeatures();
            var feature = features.FirstOrDefault(f => f.ModuleId.Equals(context.Feature.ModuleId, StringComparison.OrdinalIgnoreCase));
            if (feature != null)
            {
                feature.FeatureSettings = new ShellFeatureSettings()
                {
                    DisplayText = "Search"
                };

                // Persist changes
                await _shellFeatureStore.UpdateAsync(feature);

            }

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
