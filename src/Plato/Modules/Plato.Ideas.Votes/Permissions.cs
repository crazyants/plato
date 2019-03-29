using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Votes
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission VoteQuestions =
            new Permission("VoteQuestions", "Can up & down vote questions");

        public static readonly Permission VoteAnswers =
            new Permission("VoteAnswers", "Can up & down vote answers");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                VoteQuestions,
                VoteAnswers
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
                        VoteQuestions,
                        VoteAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        VoteQuestions,
                        VoteAnswers
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        VoteQuestions,
                        VoteAnswers
                    }
                }
            };
        }

    }

}
