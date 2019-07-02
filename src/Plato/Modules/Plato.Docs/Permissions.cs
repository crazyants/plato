using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostDocs =
            new Permission("PostDocs", "Post docs");

        public static readonly Permission PostDocComments =
            new Permission("PostDocComments", "Post comments to docs");
        
        public static readonly Permission EditOwnDocs =
            new Permission("EditOwnDocs", "Edit own docs");

        public static readonly Permission EditAnyDoc =
            new Permission("EditAnyDoc", "Edit any doc");

        public static readonly Permission SortOwnDocs =
            new Permission("SortOwnDocs", "Sort own docs");

        public static readonly Permission SortAnyDoc =
            new Permission("SortAnyDoc", "Sort any doc");
        
        public static readonly Permission EditOwnDocComments =
            new Permission("EditOwnDocComments", "Edit own comments");

        public static readonly Permission EditAnyDocComment =
            new Permission("EditAnyDocComment", "Edit any comment");
        
        public static readonly Permission DeleteOwnDocs = 
            new Permission("DeleteOwnDocs", "Soft delete own docs");

        public static readonly Permission RestoreOwnDocs =
            new Permission("RestoreOwnDocs", "Restore own soft deleted docs");

        public static readonly Permission PermanentDeleteOwnDocs =
            new Permission("PermanentDeleteOwnDocs", "Permanently delete own docs");

        public static readonly Permission DeleteAnyDoc =
            new Permission("DeleteAnyDoc", "Soft delete any doc");

        public static readonly Permission RestoreAnyDoc =
            new Permission("RestoreAnyDoc", "Restore any soft deleted doc");

        public static readonly Permission PermanentDeleteAnyDoc =
            new Permission("PermanentDeleteAnyDoc", "Permanently delete any doc");

        public static readonly Permission ViewDeletedDocs =
            new Permission("ViewDeletedDocs", "View soft deleted docs");
        
        public static readonly Permission DeleteOwnDocComments =
            new Permission("DeleteOwnDocComments", "Soft delete own comments");

        public static readonly Permission RestoreOwnDocComments =
            new Permission("RestoreOwnDocComments", "Restore own soft deleted comments");

        public static readonly Permission PermanentDeleteOwnDocComments =
            new Permission("PermanentDeleteOwnDocComments", "Permanently delete own comments");

        public static readonly Permission DeleteAnyDocComment =
            new Permission("DeleteAnyDocComment", "Soft delete any comment");

        public static readonly Permission RestoreAnyDocComment =
            new Permission("RestoreAnyDocComment", "Restore any soft deleted comment");

        public static readonly Permission PermanentDeleteAnyDocComment =
            new Permission("PermanentDeleteAnyDocComment", "Permanently delete any comment");

        public static readonly Permission ViewDeletedDocComments =
            new Permission("ViewDeletedDocComments", "View soft deleted comments");

        public static readonly Permission ReportDocs =
            new Permission("ReportDocs", "Report docs");

        public static readonly Permission ReportDocComments =
            new Permission("ReportDocComments", "Report doc comments");
        
        public static readonly Permission PinDocs =
            new Permission("PinDocs", "Pin docs");

        public static readonly Permission UnpinDocs =
            new Permission("UnpinDocs", "Unpin docs");

        public static readonly Permission LockDocs =
            new Permission("LockDocs", "Lock docs");

        public static readonly Permission UnlockDocs =
            new Permission("UnlockDocs", "Unlock docs");

        public static readonly Permission HideDocs =
            new Permission("HideDocs", "Hide docs");

        public static readonly Permission ShowDocs =
            new Permission("ShowDocs", "Unhide docs");
        
        public static readonly Permission ViewHiddenDocs =
            new Permission("ViewHiddenDocs", "View hidden docs");

        public static readonly Permission ViewPrivateDocs =
            new Permission("ViewPrivateDocs", "View private docs");

        public static readonly Permission HideDocComments =
            new Permission("HideDocComments", "Hide comments");

        public static readonly Permission ShowDocComments =
            new Permission("ShowDocComments", "Unhide comments");
        
        public static readonly Permission ViewHiddenDocComments =
            new Permission("ViewHiddenDocComments", "View hidden comments");

        public static readonly Permission DocToSpam =
            new Permission("DocToSpam", "Move docs to SPAM");

        public static readonly Permission DocFromSpam =
            new Permission("DocFromSpam", "Remove docs from SPAM");
        
        public static readonly Permission ViewSpamDocs =
            new Permission("ViewSpamDocs", "View docs flagged as SPAM");
        
        public static readonly Permission DocCommentToSpam =
            new Permission("DocCommentToSpam", "Move comments to SPAM");

        public static readonly Permission DocCommentFromSpam =
            new Permission("DocCommentFromSpam", "Remove comments from SPAM");

        public static readonly Permission ViewSpamDocComments =
            new Permission("ViewSpamDocComments", "View comments flagged as SPAM");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostDocs,
                PostDocComments,
                EditOwnDocs,
                EditAnyDoc,
                SortOwnDocs,
                SortAnyDoc,
                EditOwnDocComments,
                EditAnyDocComment,
                DeleteOwnDocs,
                RestoreOwnDocs,
                PermanentDeleteOwnDocs,
                DeleteAnyDoc,
                RestoreAnyDoc,
                PermanentDeleteAnyDoc,
                DeleteOwnDocComments,
                RestoreOwnDocComments,
                PermanentDeleteOwnDocComments,
                DeleteAnyDocComment,
                RestoreAnyDocComment,
                PermanentDeleteAnyDocComment,
                ReportDocs,
                ReportDocComments,
                ViewDeletedDocs,
                ViewDeletedDocComments,
                PinDocs,
                UnpinDocs,
                LockDocs,
                UnlockDocs,
                HideDocs,
                ShowDocs,
                ViewHiddenDocs,
                ViewPrivateDocs,
                HideDocComments,
                ShowDocComments,
                ViewHiddenDocComments,
                DocToSpam,
                DocFromSpam,
                ViewSpamDocs,
                DocCommentToSpam,
                DocCommentFromSpam,
                ViewSpamDocComments
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
                        PostDocs,
                        PostDocComments,
                        EditOwnDocs,
                        EditAnyDoc,
                        SortOwnDocs,
                        SortAnyDoc,
                        EditOwnDocComments,
                        EditAnyDocComment,
                        DeleteOwnDocs,
                        RestoreOwnDocs,
                        PermanentDeleteOwnDocs,
                        DeleteAnyDoc,
                        RestoreAnyDoc,
                        PermanentDeleteAnyDoc,
                        ViewDeletedDocs,
                        DeleteOwnDocComments,
                        RestoreOwnDocComments,
                        PermanentDeleteOwnDocComments,
                        DeleteAnyDocComment,
                        RestoreAnyDocComment,
                        PermanentDeleteAnyDocComment,
                        ViewDeletedDocComments,
                        ReportDocs,
                        ReportDocComments,
                        PinDocs,
                        UnpinDocs,
                        LockDocs,
                        UnlockDocs,
                        HideDocs,
                        ShowDocs,
                        ViewHiddenDocs,
                        ViewPrivateDocs,
                        HideDocComments,
                        ShowDocComments,
                        ViewHiddenDocComments,
                        DocToSpam,
                        DocFromSpam,
                        ViewSpamDocs,
                        DocCommentToSpam,
                        DocCommentFromSpam,
                        ViewSpamDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostDocComments,
                        EditOwnDocComments,
                        DeleteOwnDocComments,
                        ReportDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostDocs,
                        PostDocComments,
                        EditOwnDocs,
                        EditAnyDoc,
                        SortOwnDocs,
                        SortAnyDoc,
                        EditOwnDocComments,
                        EditAnyDocComment,
                        DeleteOwnDocs,
                        RestoreOwnDocs,
                        PermanentDeleteOwnDocs,
                        DeleteAnyDoc,
                        RestoreAnyDoc,
                        PermanentDeleteAnyDoc,
                        ViewDeletedDocs,
                        DeleteOwnDocComments,
                        RestoreOwnDocComments,
                        PermanentDeleteOwnDocComments,
                        DeleteAnyDocComment,
                        RestoreAnyDocComment,
                        PermanentDeleteAnyDocComment,
                        ViewDeletedDocComments,
                        ReportDocs,
                        ReportDocComments,
                        PinDocs,
                        UnpinDocs,
                        LockDocs,
                        UnlockDocs,
                        HideDocs,
                        ShowDocs,
                        ViewHiddenDocs,
                        ViewPrivateDocs,
                        HideDocComments,
                        ShowDocComments,
                        ViewHiddenDocComments,
                        DocToSpam,
                        DocFromSpam,
                        ViewSpamDocs,
                        DocCommentToSpam,
                        DocCommentFromSpam,
                        ViewSpamDocComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportDocs,
                        ReportDocComments
                    }
                }
            };
        }

    }

}
