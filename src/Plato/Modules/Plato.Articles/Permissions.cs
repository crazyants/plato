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

        public static readonly Permission ViewDeletedArticles =
            new Permission("ViewDeletedArticles", "View deleted articles");
        
        public static readonly Permission DeleteOwnArticleComments =
            new Permission("DeleteOwnArticleComments", "Delete own comments");

        public static readonly Permission RestoreOwnArticleComments =
            new Permission("RestoreOwnArticleComments", "Restore own comments");
        
        public static readonly Permission DeleteAnyArticleComment =
            new Permission("DeleteAnyArticleComment", "Delete any comment");

        public static readonly Permission RestoreAnyArticleComment =
            new Permission("RestoreAnyArticleComment", "Restore any comment");
        
        public static readonly Permission ViewDeletedArticleComments =
            new Permission("ViewDeletedArticleComments", "View deleted comments");
        
        public static readonly Permission ReportArticles =
            new Permission("ReportArticles", "Report articles");

        public static readonly Permission ReportArticleComments =
            new Permission("ReportArticleComments", "Report comments");
        
        public static readonly Permission PinArticles =
            new Permission("PinArticles", "Pin articles");

        public static readonly Permission UnpinArticles =
            new Permission("UnpinArticles", "Unpin articles");

        public static readonly Permission LockArticles =
            new Permission("LockArticles", "Lock articles");

        public static readonly Permission UnlockArticles =
            new Permission("UnlockArticles", "Unlock articles");

        public static readonly Permission HideArticles =
            new Permission("HideArticles", "Hide articles");

        public static readonly Permission ShowArticles =
            new Permission("ShowArticles", "Unhide articles");

        public static readonly Permission ViewPrivateArticles =
            new Permission("ViewPrivateArticles", "View hidden articles");

        public static readonly Permission HideArticleComments =
            new Permission("HideArticleComments", "Hide comments");

        public static readonly Permission ShowArticleComments =
            new Permission("ShowArticleComments", "Unhide comments");
        
        public static readonly Permission ViewPrivateArticleComments =
            new Permission("ViewPrivateArticleComments", "View hidden comments");

        public static readonly Permission ArticleToSpam =
            new Permission("ArticleToSpam", "Move articles to SPAM");

        public static readonly Permission ArticleFromSpam =
            new Permission("ArticleFromSpam", "Remove articles from SPAM");

        public static readonly Permission ViewSpamArticles =
            new Permission("ViewSpamArticles", "View articles flagged as SPAM");

        public static readonly Permission ArticleCommentToSpam =
            new Permission("ArticleCommentToSpam", "Move comments to SPAM");

        public static readonly Permission ArticleCommentFromSpam =
            new Permission("ArticleCommentFromSpam", "Remove comments from SPAM");

        public static readonly Permission ViewSpamArticleComments =
            new Permission("ViewSpamArticleComments", "View comments flagged as SPAM");
        
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
                ViewDeletedArticles,
                DeleteOwnArticleComments,
                RestoreOwnArticleComments,
                DeleteAnyArticleComment,
                RestoreAnyArticleComment,
                ViewDeletedArticleComments,
                ReportArticles,
                ReportArticleComments,
                PinArticles,
                UnpinArticles,
                LockArticles,
                UnlockArticles,
                HideArticles,
                ShowArticles,
                ViewPrivateArticles,
                HideArticleComments,
                ShowArticleComments,
                ViewPrivateArticleComments,
                ArticleToSpam,
                ArticleFromSpam,
                ViewSpamArticles,
                ArticleCommentToSpam,
                ArticleCommentFromSpam,
                ViewPrivateArticleComments
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
                        ViewDeletedArticles,
                        DeleteOwnArticleComments,
                        RestoreOwnArticleComments,
                        DeleteAnyArticleComment,
                        RestoreAnyArticleComment,
                        ViewDeletedArticleComments,
                        ReportArticles,
                        ReportArticleComments,
                        PinArticles,
                        UnpinArticles,
                        LockArticles,
                        UnlockArticles,
                        HideArticles,
                        ShowArticles,
                        ViewPrivateArticles,
                        HideArticleComments,
                        ShowArticleComments,
                        ViewPrivateArticleComments,
                        ArticleToSpam,
                        ArticleFromSpam,
                        ViewSpamArticles,
                        ArticleCommentToSpam,
                        ArticleCommentFromSpam,
                        ViewPrivateArticleComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostComments,
                        EditOwnComment,
                        DeleteOwnArticleComments,
                        ReportArticles,
                        ReportArticleComments
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
                        ViewDeletedArticles,
                        DeleteOwnArticleComments,
                        RestoreOwnArticleComments,
                        ViewDeletedArticleComments,
                        ReportArticles,
                        ReportArticleComments,
                        PinArticles,
                        UnpinArticles,
                        LockArticles,
                        UnlockArticles,
                        HideArticles,
                        ShowArticles,
                        ViewPrivateArticles,
                        HideArticleComments,
                        ShowArticleComments,
                        ViewPrivateArticleComments,
                        ArticleToSpam,
                        ArticleFromSpam,
                        ViewSpamArticles,
                        ArticleCommentToSpam,
                        ArticleCommentFromSpam,
                        ViewPrivateArticleComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportArticles,
                        ReportArticleComments
                    }
                }
            };
        }

    }

}
