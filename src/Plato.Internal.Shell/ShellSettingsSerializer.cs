using System;
using Microsoft.Extensions.Configuration;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions.Models;

namespace Plato.Internal.Shell
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
