using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Models.Features;
using Plato.Internal.Stores.Abstractions.Shell;

namespace Plato.Features.Updates.Services
{
    
    public class AutomaticFeatureMigrations : IAutomaticFeatureMigrations
    {

        private readonly IShellFeatureStore<ShellFeature> _shellFeatureStore;
        private readonly IShellFeatureUpdater _shellFeatureUpdater;
        
        public AutomaticFeatureMigrations(
            IShellFeatureUpdater shellFeatureUpdater, 
            IShellFeatureStore<ShellFeature> shellFeatureStore)
        {
            _shellFeatureUpdater = shellFeatureUpdater;
            _shellFeatureStore = shellFeatureStore;
        }

        public async Task<ICommandResultBase> InitialMigrationsAsync()
        {

            // Our result
            var output = new CommandResultBase();

            // Get all installed features
            var features = await _shellFeatureStore.SelectFeatures();

            // We need features to upgrade
            if (features == null)
            {
                return output.Failed("No features could be found within the shell features store.");
            }
            
            // Attempt to update each found feature and compile any errors
            var errors = new List<CommandError>();
            foreach (var feature in features)
            {
                var result = await _shellFeatureUpdater.UpdateAsync(feature.ModuleId);
                if (result.Errors.Any())
                {
                    errors.AddRange(result.Errors);
                }
            }
            
            // Did any feature upgrade encounter errors?
            if (errors.Count > 0)
            {
                return output.Failed(errors.ToArray());
            }
            
            // Return success
            return output.Success();
            
        }

    }

}
