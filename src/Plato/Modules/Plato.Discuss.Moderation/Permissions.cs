using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Moderation
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ViewQueue =
            new Permission("ViewQueue", "Can view moderation queue");

        public static readonly Permission ApproveTopics =
            new Permission("ApproveTopics", "Can approve topics");

        public static readonly Permission RejectTopics =
            new Permission("RejectTopics", "Can reject topics");


        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ViewQueue
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
                        ViewQueue
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ViewQueue
                    }
                }
            };

        }

    }

}
