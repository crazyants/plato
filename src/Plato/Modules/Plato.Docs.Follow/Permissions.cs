using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        // FollowDocs
        // AutoFollowDocs
        // AutoFollowDocComments
        // SendDocFollows

        public static readonly Permission FollowDocs =
            new Permission("FollowDocs", "Can follow docs");

        public static readonly Permission AutoFollowDocs =
            new Permission("AutoFollowDocs", "Automatically follow when posting new docs");

        public static readonly Permission AutoFollowDocComments =
            new Permission("AutoFollowDocComments", "Automatically follow docs when posting comments");
        
        public static readonly Permission SendDocFollows =
            new Permission("SendDocFollows", "Can send follow notifications when updating docs");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowDocs,
                AutoFollowDocs,
                AutoFollowDocComments,
                SendDocFollows
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
                        FollowDocs,
                        AutoFollowDocs,
                        AutoFollowDocComments,
                        SendDocFollows
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowDocs,
                        AutoFollowDocs,
                        AutoFollowDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowDocs,
                        AutoFollowDocs,
                        AutoFollowDocComments,
                        SendDocFollows
                    }
                }
            };

        }

    }

}
