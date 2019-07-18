using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Categories.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowDocCategories =
            new Permission("FollowDocCategories", "Can follow all categories");

        public static readonly Permission FollowDocCategory =
            new Permission("FollowDocCategory", "Can follow any individual category");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowDocCategories,
                FollowDocCategory
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
                        FollowDocCategories,
                        FollowDocCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowDocCategories,
                        FollowDocCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowDocCategories,
                        FollowDocCategory
                    }
                }
            };

        }

    }

}
