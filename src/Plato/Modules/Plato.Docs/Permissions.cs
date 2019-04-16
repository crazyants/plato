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
            new Permission("EditAnyDoc", "Edit any docs");

        public static readonly Permission SortOwnDocs =
            new Permission("SortOwnDocs", "Sort own docs");

        public static readonly Permission SortAnyDoc =
            new Permission("SortAnyDoc", "Sort any docs");
        
        public static readonly Permission EditOwnDocComments =
            new Permission("EditOwnDocComments", "Edit own comments");

        public static readonly Permission EditAnyDocComment =
            new Permission("EditAnyDocComment", "Edit any comment");
        
        public static readonly Permission DeleteOwnDocs = 
            new Permission("DeleteOwnDocs", "Delete own docs");

        public static readonly Permission RestoreOwnDocs =
            new Permission("RestoreOwnDocs", "Restore own docs");
        
        public static readonly Permission DeleteAnyDoc =
            new Permission("DeleteAnyDoc", "Delete any doc");

        public static readonly Permission RestoreAnyDoc =
            new Permission("RestoreAnyDoc", "Restore any doc");

        public static readonly Permission ViewDeletedDocs =
            new Permission("ViewDeletedDocs", "View deleted doc");
        
        public static readonly Permission DeleteOwnDocComments =
            new Permission("DeleteOwnDocComments", "Delete own comments");

        public static readonly Permission RestoreOwnDocComments =
            new Permission("RestoreOwnDocComments", "Restore own comments");
        
        public static readonly Permission DeleteAnyDocComment =
            new Permission("DeleteAnyDocComment", "Delete any comment");

        public static readonly Permission RestoreAnyDocComment =
            new Permission("RestoreAnyDocComment", "Restore any comment");

        public static readonly Permission ViewDeletedDocComments =
            new Permission("ViewDeletedDocComments", "View deleted comments");

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
        
        public static readonly Permission ViewPrivateDocs =
            new Permission("ViewPrivateDocs", "View hidden docs");

        public static readonly Permission HideDocComments =
            new Permission("HideDocComments", "Hide comments");

        public static readonly Permission ShowDocComments =
            new Permission("ShowDocComments", "Unhide comments");
        
        public static readonly Permission ViewPrivateDocComments =
            new Permission("ViewPrivateDocComments", "View hidden comments");

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
                DeleteAnyDoc,
                RestoreAnyDoc,
                DeleteOwnDocComments,
                RestoreOwnDocComments,
                DeleteAnyDocComment,
                RestoreAnyDocComment,
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
                ViewPrivateDocs,
                HideDocComments,
                ShowDocComments,
                ViewPrivateDocComments,
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
                        DeleteAnyDoc,
                        RestoreAnyDoc,
                        ViewDeletedDocs,
                        DeleteOwnDocComments,
                        RestoreOwnDocComments,
                        DeleteAnyDocComment,
                        RestoreAnyDocComment,
                        ViewDeletedDocComments,
                        ReportDocs,
                        ReportDocComments,
                        PinDocs,
                        UnpinDocs,
                        LockDocs,
                        UnlockDocs,
                        HideDocs,
                        ShowDocs,
                        ViewPrivateDocs,
                        HideDocComments,
                        ShowDocComments,
                        ViewPrivateDocComments,
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
                        DeleteAnyDoc,
                        RestoreAnyDoc,
                        ViewDeletedDocs,
                        DeleteOwnDocComments,
                        RestoreOwnDocComments,
                        DeleteAnyDocComment,
                        RestoreAnyDocComment,
                        ViewDeletedDocComments,
                        ReportDocs,
                        ReportDocComments,
                        PinDocs,
                        UnpinDocs,
                        LockDocs,
                        UnlockDocs,
                        HideDocs,
                        ShowDocs,
                        ViewPrivateDocs,
                        HideDocComments,
                        ShowDocComments,
                        ViewPrivateDocComments,
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
