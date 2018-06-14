using System;

namespace Plato.Internal.Models.Shell
{
    public interface IShellSettings
    {

        string Name { get; set; }

        string Location { get; set; }

        string ConnectionString { get; set; }

        string RequestedUrlHost { get; set; }

        string RequestedUrlPrefix { get; set; }

        string TablePrefix { get; set; }
        
        string DatabaseProvider { get; set; }

        string Theme { get; set; }

        string AuthCookieName { get; }

        TenantState State { get; set; }

    }
}
