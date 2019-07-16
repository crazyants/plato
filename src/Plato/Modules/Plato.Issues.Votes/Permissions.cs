using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Votes
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission VoteIdeas =
            new Permission("VoteIssues", "Can up & down vote issues");

        public static readonly Permission VoteIdeaComments =
            new Permission("VoteIssueComments", "Can up & down vote issue comments");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                VoteIdeas,
                VoteIdeaComments
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
                        VoteIdeas,
                        VoteIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        VoteIdeas,
                        VoteIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        VoteIdeas,
                        VoteIdeaComments
                    }
                }
            };
        }

    }

}
