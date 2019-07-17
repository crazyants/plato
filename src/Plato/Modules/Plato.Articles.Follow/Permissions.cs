using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowArticles =
            new Permission("FollowArticles", "Can follow articles");

        public static readonly Permission FollowNewArticles =
            new Permission("FollowNewArticles", "Automatically follow new articles");

        public static readonly Permission FollowParticipatedArticles =
            new Permission("FollowParticipatedArticles", "Automatically follow articles when posting comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowArticles,
                FollowNewArticles,
                FollowParticipatedArticles
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
                        FollowArticles,
                        FollowNewArticles,
                        FollowParticipatedArticles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowArticles,
                        FollowNewArticles,
                        FollowParticipatedArticles
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowArticles,
                        FollowNewArticles,
                        FollowParticipatedArticles
                    }
                }
            };

        }

    }

}
