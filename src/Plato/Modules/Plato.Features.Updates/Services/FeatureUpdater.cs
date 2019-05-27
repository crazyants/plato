using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Data.Migrations.Abstractions;
using Plato.Internal.Features.Abstractions;

namespace Plato.Features.Updates.Services
{

    public class FeatureUpdater : IFeatureUpdater
    {

        private readonly IOptions<PlatoOptions> _platoOptions;
        private readonly IFeatureFacade _featureFacade;
        private readonly IShellDescriptorManager _shellDescriptorManager;
        private readonly IDataMigrationBuilder _migrationBuilder;

        public FeatureUpdater(
            IOptions<PlatoOptions> platoOptions,
            IFeatureFacade featureFacade,
            IDataMigrationBuilder migrationBuilder,
            IShellDescriptorManager shellDescriptorManager)
        {
            _platoOptions = platoOptions;
            _featureFacade = featureFacade;
            _migrationBuilder = migrationBuilder;
            _shellDescriptorManager = shellDescriptorManager;
        }

        public async Task<ICommandResultBase> UpdateAsync(string moduleId)
        {

            // The result to return
            var result = new CommandResultBase();

            // Get installed shell feature
            var feature = await _featureFacade.GetFeatureByIdAsync(moduleId);

            // Ensure we found the installed feature 
            if (feature == null)
            {
                return result.Failed($"A feature with Id '{moduleId}' could not be found within the ShellFeatures table.");
            }
            
            // Get available module
            var module = await _shellDescriptorManager.GetFeatureAsync(moduleId);
            
            // Ensure we found the module 
            if (module == null)
            {
                return result.Failed($"A module with Id '{moduleId}' could not be found on the file system.");
            }
            
            // 1. Check to ensure the module we are updating is compatible 
            // with the current version of Plato we are running

            var modulePlatoVersion = module.Descriptor.PlatoVersion.ToVersion();

            // Does the module have a Plato version defined?
            if (modulePlatoVersion != null)
            {
                // Get current plato version
                var currentPlatoVersion = _platoOptions.Value.Version.ToVersion();
                if (currentPlatoVersion != null)
                {
                    // Does the module require a newer version of Plato?
                    if (modulePlatoVersion > currentPlatoVersion)
                    {
                        return result.Failed($"{moduleId} requires Plato {modulePlatoVersion.ToString()} whilst you are using {currentPlatoVersion.ToString()}. The feature cannot be updated.");
                    }
                }

            }
            
            // 2. Perform migrations from current installed feature version
            // to latest available migration available within modules IMigrationProvider

            var from = feature.Version.ToVersion();
            var to = module.Descriptor.Version.ToVersion();

            if (from == null)
            {
                return result.Failed(
                    $"Could not convert version for feature {feature.ModuleId} of {feature.Version} to a valid version object. Please check the version within the ShellFeatures database table.");
            }
            
            if (to == null)
            {
                return result.Failed(
                    $"Could not convert version for module {module.Descriptor.Id} of {module.Descriptor.Version} to a valid version object. Please check the version within the modules manifest file.");
            }

            // Build migrations for feature & versions
            var migrations = _migrationBuilder.BuildMigrations(moduleId, from, to);
            
            // Apply migrations
            var migrationResults =  await migrations.ApplyMigrationsAsync();

            // Did any errors occur whilst applying the migration?
            if (migrationResults.Errors.Any())
            {
                var errors = new List<CommandError>();
                foreach (var error in migrationResults.Errors)
                {
                    errors.Add(new CommandError(error.Message));
                }
                return result.Failed(errors.ToArray());
            }

            // 3. Add newly added missing permissions to existing roles via IDefaultRolesManager

            // 4. Upon success update version in ShellFeatures + ShellDescriptor

            return result.Success();

        }

    }

}
