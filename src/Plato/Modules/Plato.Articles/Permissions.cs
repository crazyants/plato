using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Articles
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostArticles =
            new Permission("PostArticles", "Post articles");

        public static readonly Permission PostComments =
            new Permission("PostComments", "Post comments");

        public static readonly Permission EditOwnArticles =
            new Permission("EditOwnArticles", "Edit own topics");

        public static readonly Permission EditAnyArticle =
            new Permission("EditAnyArticle", "Edit own topics");
        
        public static readonly Permission EditOwnComment =
            new Permission("EditOwnComment", "Edit own replies");

        public static readonly Permission EditAnyComment =
            new Permission("EditAnyComment", "Edit any reply");
        
        public static readonly Permission DeleteOwnArticles = 
            new Permission("DeleteOwnArticles", "Delete own topics");

        public static readonly Permission RestoreOwnArticles =
            new Permission("RestoreOwnArticles", "Restore own topics");
        
        public static readonly Permission DeleteAnyArticle =
            new Permission("DeleteAnyArticle", "Delete any topic");

        public static readonly Permission RestoreAnyArticle =
            new Permission("RestoreAnyArticle", "Restore any topic");
        
        public static readonly Permission DeleteOwnComments =
            new Permission("DeleteOwnComments", "Delete own replies");

        public static readonly Permission RestoreOwnComments =
            new Permission("RestoreOwnComments", "Restore own replies");
        
        public static readonly Permission DeleteAnyComment =
            new Permission("DeleteAnyComment", "Delete any reply");

        public static readonly Permission RestoreAnyComment =
            new Permission("RestoreAnyComment", "Restore any reply");

        public static readonly Permission ReportArticles =
            new Permission("ReportArticles", "Report topics");

        public static readonly Permission ReportComments =
            new Permission("ReportComments", "Report replies");

        public static readonly Permission ViewPrivateArticles =
            new Permission("ViewPrivateArticles", "View hidden topics");

        public static readonly Permission ViewPrivateComments =
            new Permission("ViewPrivateComments", "View hidden replies");

        public static readonly Permission ViewSpamArticles =
            new Permission("ViewSpamArticles", "View topics flagged as SPAM");

        public static readonly Permission ViewSpamComments =
            new Permission("ViewSpamReplies", "View replies flagged as SPAM");
        
        public static readonly Permission ViewDeletedArticles =
            new Permission("ViewDeletedTopics", "View deleted topics");

        public static readonly Permission ViewDeletedComments =
            new Permission("ViewDeletedReplies", "View deleted replies");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostArticles,
                PostComments,
                EditOwnArticles,
                EditAnyArticle,
                EditOwnComment,
                EditAnyComment,
                DeleteOwnArticles,
                RestoreOwnArticles,
                DeleteAnyArticle,
                RestoreAnyArticle,
                DeleteOwnComments,
                RestoreOwnComments,
                DeleteAnyComment,
                RestoreAnyComment,
                ReportArticles,
                ReportComments,
                ViewPrivateArticles,
                ViewPrivateComments,
                ViewSpamArticles,
                ViewSpamComments,
                ViewDeletedArticles,
                ViewDeletedComments
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
                        PostArticles,
                        PostComments,
                        EditOwnArticles,
                        EditAnyArticle,
                        EditOwnComment,
                        EditAnyComment,
                        DeleteOwnArticles,
                        RestoreOwnArticles,
                        DeleteAnyArticle,
                        RestoreAnyArticle,
                        DeleteOwnComments,
                        RestoreOwnComments,
                        DeleteAnyComment,
                        RestoreAnyComment,
                        ReportArticles,
                        ReportComments,
                        ViewPrivateArticles,
                        ViewPrivateComments,
                        ViewSpamArticles,
                        ViewSpamComments,
                        ViewDeletedArticles,
                        ViewDeletedComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostComments,
                        EditOwnArticles,
                        EditOwnComment,
                        DeleteOwnArticles,
                        DeleteOwnComments,
                        ReportArticles,
                        ReportComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostArticles,
                        PostComments,
                        EditOwnArticles,
                        EditOwnComment,
                        DeleteOwnArticles,
                        RestoreOwnArticles,
                        DeleteOwnComments,
                        RestoreOwnComments,
                        ReportArticles,
                        ReportComments,
                        ViewPrivateArticles,
                        ViewPrivateComments,
                        ViewSpamArticles,
                        ViewSpamComments,
                        ViewDeletedArticles,
                        ViewDeletedComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportArticles,
                        ReportComments
                    }
                }
            };
        }

    }

}
