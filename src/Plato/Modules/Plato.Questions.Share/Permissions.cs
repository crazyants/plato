using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Share
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission ShareQuestions =
            new Permission("ShareQuestions", "Share questions");

        public static readonly Permission ShareComments =
            new Permission("ShareAnswers", "Share question answers");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ShareQuestions,
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
                        ShareQuestions,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        ShareQuestions,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ShareQuestions,
                        ShareComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        ShareQuestions,
                        ShareComments
                    }
                }
            };

        }

    }

}
