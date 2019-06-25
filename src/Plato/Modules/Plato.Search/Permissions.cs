using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Search
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ManageSearchSettings =
            new Permission("ManageSearchSettings", "Manage search settings");

        public static readonly Permission SearchSpam =
            new Permission("SearchSpam", "Search spam");

        public static readonly Permission SearchHidden =
            new Permission("SearchHidden", "Search hidden");

        public static readonly Permission SearchDeleted =
            new Permission("SearchDeleted", "Search deleted");

        public static readonly Permission SearchPrivate =
            new Permission("SearchPrivate", "Search private");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageSearchSettings,
                SearchSpam,
                SearchHidden,
                SearchDeleted,
                SearchPrivate
            };
        }

        public IEnumerable<DefaultPermissions<Permission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        ManageSearchSettings,
                        SearchSpam,
                        SearchHidden,
                        SearchDeleted,
                        SearchPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        SearchSpam,
                        SearchHidden,
                        SearchDeleted,
                        SearchPrivate
                    }
                }
            };

        }

    }

}
