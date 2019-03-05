using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Data.Schemas.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Security.Abstractions;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Discuss.History.Handlers
{
    
    public class FeatureEventHandler : BaseFeatureEventHandler
    {
  
        private readonly IDefaultRolesManager _defaultRolesManager;

        public FeatureEventHandler(
            ISchemaBuilder schemaBuilder,
            IShellFeatureStore<ShellFeature> shellFeatureStore,
            IDefaultRolesManager defaultRolesManager)
        {
            _defaultRolesManager = defaultRolesManager;
        }
        
        #region "Implementation"

        public override Task InstallingAsync(IFeatureEventContext context)
        {
            return Task.CompletedTask;
        }

        public override async Task InstalledAsync(IFeatureEventContext context)
        {
            // Apply default permissions to default roles for new feature
            await _defaultRolesManager.UpdateDefaultRolesAsync(new Permissions());
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
        
    }

}
