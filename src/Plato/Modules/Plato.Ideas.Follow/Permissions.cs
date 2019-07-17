using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {
        
        // FollowIdeas
        // AutoFollowIdeas
        // AutoFollowIdeaComments
        
        public static readonly Permission FollowIdeas =
            new Permission("FollowIdeas", "Can follow ideas");

        public static readonly Permission AutoFollowIdeas =
            new Permission("AutoFollowIdeas", "Automatically follow when posting new ideas");

        public static readonly Permission AutoFollowIdeaComments =
            new Permission("AutoFollowIdeaComments", "Automatically follow ideas when posting replies");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowIdeas,
                AutoFollowIdeas,
                AutoFollowIdeaComments
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
                        AutoFollowIdeas,
                        AutoFollowIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowIdeas,
                        AutoFollowIdeas,
                        AutoFollowIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowIdeas,
                        AutoFollowIdeas,
                        AutoFollowIdeaComments
                    }
                }
            };

        }

    }

}
