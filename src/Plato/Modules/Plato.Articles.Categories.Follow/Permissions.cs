using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Categories.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowArticleCategories =
            new Permission("FollowArticleCategories", "Can follow all categories");

        public static readonly Permission FollowArticleCategory =
            new Permission("FollowArticleCategory", "Can follow any individual category");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowArticleCategories,
                FollowArticleCategory
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
                        FollowArticleCategories,
                        FollowArticleCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowArticleCategories,
                        FollowArticleCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowArticleCategories,
                        FollowArticleCategory
                    }
                }
            };

        }

    }

}
