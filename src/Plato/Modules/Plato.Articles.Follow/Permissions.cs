using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        // FollowArticles
        // AutoFollowArticles
        // AutoFollowArticleComments
        // SendArticleFollows

        public static readonly Permission FollowArticles =
            new Permission("FollowArticles", "Can follow articles");

        public static readonly Permission AutoFollowArticles =
            new Permission("AutoFollowArticles", "Automatically follow when posting new articles");

        public static readonly Permission AutoFollowArticleComments =
            new Permission("AutoFollowArticleComments", "Automatically follow articles when posting comments");
        
        public static readonly Permission SendArticleFollows =
            new Permission("SendArticleFollows", "Can send follow notifications when updating articles");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowArticles,
                AutoFollowArticles,
                AutoFollowArticleComments,
                SendArticleFollows
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
                        AutoFollowArticles,
                        AutoFollowArticleComments,
                        SendArticleFollows
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowArticles,
                        AutoFollowArticles,
                        AutoFollowArticleComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowArticles,
                        AutoFollowArticles,
                        AutoFollowArticleComments,
                        SendArticleFollows
                    }
                }
            };

        }

    }

}
