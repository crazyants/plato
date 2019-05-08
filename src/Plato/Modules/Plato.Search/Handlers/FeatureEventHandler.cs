using System;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Search.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        public string Version { get; } = "1.0.0";

        private readonly ISchemaBuilder _schemaBuilder;
        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;
        private readonly IDefaultRolesManager _defaultRolesManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            IDefaultRolesManager defaultRolesManager)
        {
            _schemaBuilder = schemaBuilder;
            _shellFeatureStore = shellFeatureStore;
            _defaultRolesManager = defaultRolesManager;
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            await InitializeDefaultFeatureSettings(context);
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
        
        async Task InitializeDefaultFeatureSettings(IFeatureEventContext context)
        {

            var features = await _shellFeatureStore.SelectFeatures();
            var feature = features.FirstOrDefault(f => f.ModuleId.Equals(context.Feature.ModuleId, StringComparison.OrdinalIgnoreCase));
            if (feature != null)
            {
                feature.FeatureSettings = new FeatureSettings()
                {
                    Title = "Search",
                    Description = "Search Help & Support",
                    IconCss = "fal fa-search"
                };

                // Persist changes
                await _shellFeatureStore.UpdateAsync(feature);

            }
            
            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());

        }

        #endregion

    }
}
