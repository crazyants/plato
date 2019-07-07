using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Star
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission StarQuestions =
            new Permission("StarQuestions", "Star questions");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                StarQuestions
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
                        StarQuestions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        StarQuestions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        StarQuestions
                    }
                }
            };

        }

    }

}
