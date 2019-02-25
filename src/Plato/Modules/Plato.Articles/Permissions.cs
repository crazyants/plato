using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission CreateArticles =
            new Permission("CreateArticles", "Create articles");

        public static readonly Permission EditArticles =
            new Permission("EditOwnTopics", "Edit own topics");

        public static readonly Permission PostComments =
            new Permission("PostComments", "Post comments");
        

        public static readonly Permission EditAnyTopic =
            new Permission("EditAnyTopic", "Edit any topic");

        public static readonly Permission EditOwnComment =
            new Permission("EditOwnReplies", "Edit own replies");

        public static readonly Permission EditAnyComment =
            new Permission("EditAnyReply", "Edit any reply");
        
        public static readonly Permission DeleteArticles = 
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
                CreateArticles,
                PostComments,
                EditArticles,
                EditAnyTopic,
                EditOwnComment,
                EditAnyComment,
                DeleteArticles,
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
                        CreateArticles,
                        PostComments,
                        EditArticles,
                        EditAnyTopic,
                        EditOwnComment,
                        EditAnyComment,
                        DeleteArticles,
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
                        PostComments,
                        EditArticles,
                        EditOwnComment,
                        DeleteArticles,
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
                        CreateArticles,
                        PostComments,
                        EditArticles,
                        EditOwnComment,
                        DeleteArticles,
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
