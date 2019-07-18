using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Categories.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowIdeaCategories =
            new Permission("FollowIdeaCategories", "Can follow all categories");

        public static readonly Permission FollowIdeaCategory =
            new Permission("FollowIdeaCategory", "Can follow any individual category");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowIdeaCategories,
                FollowIdeaCategory
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
                        FollowIdeaCategories,
                        FollowIdeaCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowIdeaCategories,
                        FollowIdeaCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowIdeaCategories,
                        FollowIdeaCategory
                    }
                }
            };

        }

    }

}
