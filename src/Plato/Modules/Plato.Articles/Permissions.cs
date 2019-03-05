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
            new Permission("EditOwnArticles", "Edit own articles");

        public static readonly Permission EditAnyArticle =
            new Permission("EditAnyArticle", "Edit any article");
        
        public static readonly Permission EditOwnComment =
            new Permission("EditOwnComment", "Edit own comments");

        public static readonly Permission EditAnyComment =
            new Permission("EditAnyComment", "Edit any comment");
        
        public static readonly Permission DeleteOwnArticles = 
            new Permission("DeleteOwnArticles", "Delete own articles");

        public static readonly Permission RestoreOwnArticles =
            new Permission("RestoreOwnArticles", "Restore own articles");
        
        public static readonly Permission DeleteAnyArticle =
            new Permission("DeleteAnyArticle", "Delete any article");

        public static readonly Permission RestoreAnyArticle =
            new Permission("RestoreAnyArticle", "Restore any article");
        
        public static readonly Permission DeleteOwnComments =
            new Permission("DeleteOwnComments", "Delete own comments");

        public static readonly Permission RestoreOwnComments =
            new Permission("RestoreOwnComments", "Restore own comments");
        
        public static readonly Permission DeleteAnyComment =
            new Permission("DeleteAnyComment", "Delete any comment");

        public static readonly Permission RestoreAnyComment =
            new Permission("RestoreAnyComment", "Restore any comment");

        public static readonly Permission ReportArticles =
            new Permission("ReportArticles", "Report articles");

        public static readonly Permission ReportComments =
            new Permission("ReportComments", "Report comments");

        public static readonly Permission ViewPrivateArticles =
            new Permission("ViewPrivateArticles", "View private articles");

        public static readonly Permission ViewPrivateComments =
            new Permission("ViewPrivateComments", "View private comments");

        public static readonly Permission ViewSpamArticles =
            new Permission("ViewSpamArticles", "View articles flagged as SPAM");

        public static readonly Permission ViewSpamComments =
            new Permission("ViewSpamComments", "View comments flagged as SPAM");
        
        public static readonly Permission ViewDeletedArticles =
            new Permission("ViewDeletedArticles", "View deleted articles");

        public static readonly Permission ViewDeletedComments =
            new Permission("ViewDeletedComments", "View deleted comments");
        
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
                        EditOwnComment,
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
