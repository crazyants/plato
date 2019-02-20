using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostTopics =
            new Permission("PostTopics", "Post topics");

        public static readonly Permission PostReplies =
            new Permission("PostReplies", "Post Replies");
        
        public static readonly Permission EditOwnTopics =
            new Permission("EditOwnTopics", "Edit own topics");

        public static readonly Permission EditAnyTopic =
            new Permission("EditAnyTopic", "Edit any topic");

        public static readonly Permission EditOwnReplies =
            new Permission("EditOwnReplies", "Edit own replies");

        public static readonly Permission EditAnyReply =
            new Permission("EditAnyReply", "Edit any reply");
        
        public static readonly Permission DeleteOwnTopics = 
            new Permission("DeleteOwnTopics", "Delete own topics");

        public static readonly Permission RestoreOwnTopics =
            new Permission("RestoreOwnTopics", "Restore own topics");


        public static readonly Permission DeleteAnyTopic =
            new Permission("DeleteAnyTopic", "Delete any topic");

        public static readonly Permission RestoreAnyTopic =
            new Permission("RestoreAnyTopic", "Restore any topic");
        
        public static readonly Permission DeleteOwnReplies =
            new Permission("DeleteOwnReplies", "Delete own replies");

        public static readonly Permission RestoreOwnReplies =
            new Permission("RestoreOwnReplies", "Restore own replies");
        
        public static readonly Permission DeleteAnyReply =
            new Permission("DeleteAnyReply", "Delete any reply");

        public static readonly Permission RestoreAnyReply =
            new Permission("RestoreAnyReply", "Restore any reply");

        public static readonly Permission ReportTopics =
            new Permission("ReportTopics", "Report topics");

        public static readonly Permission ReportReplies =
            new Permission("ReportReplies", "Report replies");

        public static readonly Permission ViewPrivateTopics =
            new Permission("ViewPrivateTopics", "View hidden topics");

        public static readonly Permission ViewPrivateReplies =
            new Permission("ViewPrivateReplies", "View hidden replies");

        public static readonly Permission ViewSpamTopics =
            new Permission("ViewSpamTopics", "View topics flagged as SPAM");

        public static readonly Permission ViewSpamReplies =
            new Permission("ViewSpamReplies", "View replies flagged as SPAM");
        
        public static readonly Permission ViewDeletedTopics =
            new Permission("ViewDeletedTopics", "View deleted topics");

        public static readonly Permission ViewDeletedReplies =
            new Permission("ViewDeletedReplies", "View deleted replies");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostTopics,
                PostReplies,
                EditOwnTopics,
                EditAnyTopic,
                EditOwnReplies,
                EditAnyReply,
                DeleteOwnTopics,
                RestoreOwnTopics,
                DeleteAnyTopic,
                RestoreAnyTopic,
                DeleteOwnReplies,
                RestoreOwnReplies,
                DeleteAnyReply,
                RestoreAnyReply,
                ReportTopics,
                ReportReplies,
                ViewPrivateTopics,
                ViewPrivateReplies,
                ViewSpamTopics,
                ViewSpamReplies,
                ViewDeletedTopics,
                ViewDeletedReplies
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
                        PostTopics,
                        PostReplies,
                        EditOwnTopics,
                        EditAnyTopic,
                        EditOwnReplies,
                        EditAnyReply,
                        DeleteOwnTopics,
                        RestoreOwnTopics,
                        DeleteAnyTopic,
                        RestoreAnyTopic,
                        DeleteOwnReplies,
                        RestoreOwnReplies,
                        DeleteAnyReply,
                        RestoreAnyReply,
                        ReportTopics,
                        ReportReplies,
                        ViewPrivateTopics,
                        ViewPrivateReplies,
                        ViewSpamTopics,
                        ViewSpamReplies,
                        ViewDeletedTopics,
                        ViewDeletedReplies
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostTopics,
                        PostReplies,
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
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostTopics,
                        PostReplies,
                        EditOwnTopics,
                        EditOwnReplies,
                        DeleteOwnTopics,
                        RestoreOwnTopics,
                        DeleteOwnReplies,
                        RestoreOwnReplies,
                        ReportTopics,
                        ReportReplies,
                        ViewPrivateTopics,
                        ViewPrivateReplies,
                        ViewSpamTopics,
                        ViewSpamReplies,
                        ViewDeletedTopics,
                        ViewDeletedReplies
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
