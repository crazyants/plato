using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.StopForumSpam
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission AddToStopForumSpam =
            new Permission("DiscussAddToSFS", "Can submit spammers to StopForumSpam");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                AddToStopForumSpam
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
                        AddToStopForumSpam
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        AddToStopForumSpam
                    }
                }
            };

        }

    }

}
