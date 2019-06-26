using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues.Tags
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostIssueTags =
            new Permission("PostIssueTags", "Can add tags when posting issues");

        public static readonly Permission EditIssueTags =
            new Permission("EditIssueTags", "Can edit tags when editing issues");

        public static readonly Permission PostIssueCommentTags =
            new Permission("PostIssueCommentTags", "Can add tags when posting issue comments");

        public static readonly Permission EditIssueCommentTags =
            new Permission("EditIssueCommentTags", "Can edit tags when editing issue comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostIssueTags,
                EditIssueTags,
                PostIssueCommentTags,
                EditIssueCommentTags
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
                        PostIssueTags,
                        EditIssueTags,
                        PostIssueCommentTags,
                        EditIssueCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostIssueTags,
                        EditIssueTags,
                        PostIssueCommentTags,
                        EditIssueCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostIssueTags,
                        EditIssueTags,
                        PostIssueCommentTags,
                        EditIssueCommentTags
                    }
                }
            };

        }

    }

}
