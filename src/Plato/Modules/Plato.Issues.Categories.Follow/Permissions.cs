using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Categories.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowIssueCategories =
            new Permission("FollowIssueCategories", "Can follow all categories");

        public static readonly Permission FollowIssueCategory =
            new Permission("FollowIssueCategory", "Can follow any individual category");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowIssueCategories,
                FollowIssueCategory
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
                        FollowIssueCategories,
                        FollowIssueCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowIssueCategories,
                        FollowIssueCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowIssueCategories,
                        FollowIssueCategory
                    }
                }
            };

        }

    }

}
