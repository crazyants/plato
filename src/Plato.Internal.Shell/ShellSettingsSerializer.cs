using System;
using Microsoft.Extensions.Configuration;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Shell
{

    public static class ShellSettingsSerializer
    {

        public static ShellSettings ParseSettings(IConfigurationRoot configuration)
        {

            return new ShellSettings
            {
                Name = configuration["Name"],
                RequestedUrlHost = configuration["RequestedUrlHost"],
                RequestedUrlPrefix = configuration["RequestedUrlPrefix"],
                ConnectionString = configuration["ConnectionString"],
                TablePrefix = configuration["TablePrefix"],
                DatabaseProvider = configuration["DatabaseProvider"],
                Theme = configuration["Theme"],
                State = Enum.TryParse(configuration["State"], true, out TenantState state)
                    ? state
                    : TenantState.Uninitialized
            };

        }

    }

}
