using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Categories.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowDiscussCategories =
            new Permission("FollowDiscussCategories", "Can follow all categories");

        public static readonly Permission FollowDiscussCategory =
            new Permission("FollowDiscussCategory", "Can follow any individual category");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowDiscussCategories,
                FollowDiscussCategory
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
                        FollowDiscussCategories,
                        FollowDiscussCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowDiscussCategories,
                        FollowDiscussCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowDiscussCategories,
                        FollowDiscussCategory
                    }
                }
            };

        }

    }

}
