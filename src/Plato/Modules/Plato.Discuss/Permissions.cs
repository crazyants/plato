using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission EditOwnTopics =
            new Permission("EditOwnTopics", "Can edit own topics");

        public static readonly Permission EditAnyTopic =
            new Permission("EditAnyTopic", "Can edit any topic");

        public static readonly Permission EditOwnReplies =
            new Permission("EditOwnReplies", "Can edit own replies");

        public static readonly Permission EditAnyReply =
            new Permission("EditAnyReply", "Can edit any reply");
        
        public static readonly Permission DeleteOwnTopics = 
            new Permission("DeleteOwnTopics", "Can add new users");

        public static readonly Permission DeleteAnyTopic =
            new Permission("DeleteAnyTopic", "Can edit existing users");

        public static readonly Permission DeleteOwnReplies =
            new Permission("DeleteOwnReplies", "Can delete owned replies");

        public static readonly Permission DeleteAnyReply =
            new Permission("DeleteAnyReply", "Can delete any reply");

        public static readonly Permission ReportTopics =
            new Permission("ReportTopics", "Can report topics");

        public static readonly Permission ReportReplies =
            new Permission("ReportReplies", "Can report replies");


        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                EditOwnTopics,
                EditAnyTopic,
                EditOwnReplies,
                EditAnyReply,
                DeleteOwnTopics,
                DeleteAnyTopic,
                DeleteOwnReplies,
                DeleteAnyReply,
                ReportTopics,
                ReportReplies
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
                        EditOwnTopics,
                        EditAnyTopic,
                        EditOwnReplies,
                        EditAnyReply,
                        DeleteOwnTopics,
                        DeleteAnyTopic,
                        DeleteOwnReplies,
                        DeleteAnyReply,
                        ReportTopics,
                        ReportReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        EditOwnTopics,
                        EditOwnReplies,
                        DeleteOwnTopics,
                        DeleteOwnReplies,
                        ReportTopics,
                        ReportReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Employee,
                    Permissions = new[]
                    {
                        EditOwnTopics,
                        EditOwnReplies,
                        DeleteOwnTopics,
                        DeleteOwnReplies,
                        ReportTopics,
                        ReportReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportTopics,
                        ReportReplies
                    }
                }
            };

        }

    }

}
