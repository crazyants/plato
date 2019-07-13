using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Entities
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewPrivateEntities =
            new Permission("ViewPrivateEntities", "View private contributions");

        public static readonly Permission ViewHiddenEntities =
            new Permission("ViewHiddenEntities", "View hidden contributions");

        public static readonly Permission ViewSpamEntities =
            new Permission("ViewSpamEntities", "View contributions flagged as SPAM");
        
        public static readonly Permission ViewDeletedEntities =
            new Permission("ViewDeletedEntities", "View deleted contributions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewPrivateEntities,
                ViewHiddenEntities,
                ViewSpamEntities,
                ViewDeletedEntities
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
                        ViewPrivateEntities,
                        ViewHiddenEntities,
                        ViewSpamEntities,
                        ViewDeletedEntities
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ViewPrivateEntities,
                        ViewHiddenEntities,
                        ViewSpamEntities,
                        ViewDeletedEntities
                    }
                }
            };

        }

    }

}
