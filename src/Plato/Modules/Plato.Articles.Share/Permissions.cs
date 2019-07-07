using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles.Share
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ShareArticles =
            new Permission("ShareArticles", "Share topics");

        public static readonly Permission ShareComments =
            new Permission("ShareArticleComments", "Share topic replies");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ShareArticles,
                ShareComments
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
                        ShareArticles,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ShareArticles,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ShareArticles,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ShareArticles,
                        ShareComments
                    }
                }
            };

        }

    }

}
