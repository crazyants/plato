using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowIdeas =
            new Permission("FollowIdeas", "Can follow ideas");

        public static readonly Permission FollowNewIdeas =
            new Permission("FollowNewIdeas", "Automatically follow new ideas");

        public static readonly Permission FollowParticipatedIdeas =
            new Permission("FollowParticipatedIdeas", "Automatically follow ideas when posting replies");


        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowIdeas,
                FollowNewIdeas,
                FollowParticipatedIdeas
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
                        FollowIdeas,
                        FollowNewIdeas,
                        FollowParticipatedIdeas
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowIdeas,
                        FollowNewIdeas,
                        FollowParticipatedIdeas
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowIdeas,
                        FollowNewIdeas,
                        FollowParticipatedIdeas
                    }
                }
            };

        }

    }

}
