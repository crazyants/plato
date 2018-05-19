using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Hosting;
using Plato.Shell.Models;

namespace Plato.SetUp.Services
{
    public class SetUpService :ISetUpService
    {

        private readonly ShellSettings _shellSettings;
        private readonly IPlatoHost _platoHost;

        public SetUpService(
            ShellSettings shellSettings,
            IPlatoHost platoHost)
        {
            _shellSettings = shellSettings;
            _platoHost = platoHost;
        }

        public async Task<string> SetupAsync(SetUpContext context)
        {
            var shellSettings = new ShellSettings(_shellSettings.Configuration);

   
            if (string.IsNullOrEmpty(shellSettings.DatabaseProvider))
            {
                shellSettings.DatabaseProvider = context.DatabaseProvider;
                shellSettings.ConnectionString = context.DatabaseConnectionString;
                //shellSettings.TablePrefix = context.DatabaseTablePrefix;
            }

            var executionId = Guid.NewGuid().ToString("n");

            shellSettings.Location = context.SiteName;
            
            shellSettings.State = TenantState.Running;
            _platoHost.UpdateShellSettings(shellSettings);
            
            return executionId;

        }

    }
}
