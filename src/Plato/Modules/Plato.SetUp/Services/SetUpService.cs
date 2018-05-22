using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Plato.Data;
using Plato.Data.Abstractions;
using Plato.Hosting;
using Plato.Shell;
using Plato.Shell.Models;
using Plato.Data.Migrations;

namespace Plato.SetUp.Services
{
    public class SetUpService :ISetUpService
    {

        private readonly ShellSettings _shellSettings;
        private readonly IShellContextFactory _shellContextFactory;
        private readonly IPlatoHost _platoHost;

        public SetUpService(
            ShellSettings shellSettings,
            IShellContextFactory shellContextFactory,
            IPlatoHost platoHost)
        {
            _shellSettings = shellSettings;
            _shellContextFactory = shellContextFactory;
            _platoHost = platoHost;
        }

        public async Task<string> SetupAsync(SetUpContext context)
        {
            var initialState = _shellSettings.State;
            try
            {
                return await SetupInternalAsync(context);
            }
            catch (Exception ex)
            {
                context.Errors.Add(ex);
                _shellSettings.State = initialState;
                throw;
            }
        }
        
        async Task<string> SetupInternalAsync(SetUpContext context)
        {

            // Set shell state to "Initializing" so that subsequent HTTP requests are responded to with "Service Unavailable" while Orchard is setting up.
            _shellSettings.State = TenantState.Initializing;
            
            var shellSettings = new ShellSettings(_shellSettings.Configuration);
            
            if (string.IsNullOrEmpty(shellSettings.DatabaseProvider))
            {
                shellSettings.DatabaseProvider = context.DatabaseProvider;
                shellSettings.ConnectionString = context.DatabaseConnectionString;
                shellSettings.TablePrefix = context.DatabaseTablePrefix;
            }

            var executionId = Guid.NewGuid().ToString("n");

            shellSettings.Location = context.SiteName;
            
            using (var shellContext = _shellContextFactory.CreateShellContext(shellSettings))
            {
                using (var scope = shellContext.ServiceProvider.CreateScope())
                {
                    
                    using (var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>())
                    {

                        // update dbContext confirmation
                       dbContext.Configure(options =>
                       {
                           options.ConnectionString = shellSettings.ConnectionString;
                           options.DatabaseProvider = shellSettings.DatabaseProvider;
                           options.TablePrefix = shellSettings.TablePrefix;
                       });
                        
                        // perform inistal migrations
                        var automaticMigrations = scope.ServiceProvider.GetService<AutomaticDataMigrations>();
                        var initialMigration = automaticMigrations.InitialMigration();

                        // handle exceptions
                        if (initialMigration.Errors.Any())
                        {
                            foreach (var error in initialMigration.Errors)
                                context.Errors.Add(error);
                        }
                    }

                }
            }

            if (context.Errors.Count > 0)
            {
                return executionId;
            }

            shellSettings.State = TenantState.Running;
            _platoHost.UpdateShellSettings(shellSettings);
            
            return executionId;

        }

        private void InitializeDatabaseAsync()
        {
            
            //var builder = new DataMigrationBuilder();
            //builder.BuildMigrations(new List<string>() { "1.0.0" });
            
        }

        private void Upgrade()
        {

            //var builder = new DataMigrationBuilder();
            //builder.BuildMigrations(new List<string>() { "1.0.0", "1.0.1" });

        }


    }
}
