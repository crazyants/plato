using Microsoft.Extensions.Configuration;
using Plato.Shell.Models;
using System;
using Plato.Abstractions.Shell;

namespace Plato.Shell
{

    public static class ShellSettingsSerializer
    {

        public static ShellSettings ParseSettings(IConfigurationRoot configuration)
        {
            var shellSettings = new ShellSettings();
            shellSettings.Name = configuration["Name"];            
            shellSettings.RequestedUrlHost = configuration["RequestedUrlHost"];
            shellSettings.RequestedUrlPrefix = configuration["RequestedUrlPrefix"];
            shellSettings.ConnectionString = configuration["ConnectionString"];
            shellSettings.TablePrefix = configuration["TablePrefix"];
            shellSettings.DatabaseProvider = configuration["DatabaseProvider"];
            shellSettings.Theme = configuration["Theme"];
            shellSettings.State = Enum.TryParse(configuration["State"], true, out TenantState state) ? state : TenantState.Uninitialized;
            return shellSettings;

        }

    }

}
