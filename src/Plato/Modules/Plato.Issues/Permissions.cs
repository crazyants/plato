using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Issues
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostIssues =
            new Permission("PostIssues", "Post issues");

        public static readonly Permission PostIssueComments =
            new Permission("PostIssueComments", "Post comments");

        public static readonly Permission EditOwnIssues =
            new Permission("EditOwnIssues", "Edit own issues");

        public static readonly Permission EditAnyIssue =
            new Permission("EditAnyIssue", "Edit any issue");
        
        public static readonly Permission EditOwnIssueComments =
            new Permission("EditOwnIssueComments", "Edit own comments");

        public static readonly Permission EditAnyIssueComment =
            new Permission("EditAnyIssueComment", "Edit any comment");
        
        public static readonly Permission DeleteOwnIssues = 
            new Permission("DeleteOwnIssues", "Soft delete own issues");

        public static readonly Permission RestoreOwnIssues =
            new Permission("RestoreOwnIssues", "Restore own soft deleted issues");

        public static readonly Permission PermanentDeleteOwnIssues =
            new Permission("PermanentDeleteOwnIssues", "Permanently delete own issues");

        public static readonly Permission DeleteAnyIssue =
            new Permission("DeleteAnyIssue", "Soft delete any issue");

        public static readonly Permission RestoreAnyIssue =
            new Permission("RestoreAnyIssue", "Restore any soft deleted issue");

        public static readonly Permission PermanentDeleteAnyIssue =
            new Permission("PermanentDeleteAnyIssue", "Permanently delete any issue");

        public static readonly Permission ViewDeletedIssues =
            new Permission("ViewDeletedIssues", "View soft deleted issues");
        
        public static readonly Permission DeleteOwnIssueComments =
            new Permission("DeleteOwnIssueComments", "Soft delete own comments");

        public static readonly Permission RestoreOwnIssueComments =
            new Permission("RestoreOwnIssueComments", "Restore own soft deleted comments");

        public static readonly Permission PermanentDeleteOwnIssueComments =
            new Permission("PermanentDeleteOwnIssueComments", "Permanently delete own comments");
        
        public static readonly Permission DeleteAnyIssueComment =
            new Permission("DeleteAnyIssueComment", "Soft delete any comment");

        public static readonly Permission RestoreAnyIssueComment =
            new Permission("RestoreAnyIssueComment", "Restore any soft deleted comment");

        public static readonly Permission PermanentDeleteAnyIssueComment =
            new Permission("PermanentDeleteAnyIssueComment", "Permanently delete any comment");
        
        public static readonly Permission ViewDeletedIssueComments =
            new Permission("ViewDeletedIssueComments", "View soft deleted comments");
        
        public static readonly Permission ReportIssues =
            new Permission("ReportIssues", "Report issues");

        public static readonly Permission ReportIssueComments =
            new Permission("ReportIssueComments", "Report comments");
        
        public static readonly Permission PinIssues =
            new Permission("PinIssues", "Pin issues");

        public static readonly Permission UnpinIssues =
            new Permission("UnpinIssues", "Unpin issues");

        public static readonly Permission LockIssues =
            new Permission("LockIssues", "Lock issues");

        public static readonly Permission UnlockIssues =
            new Permission("UnlockIssues", "Unlock issues");

        public static readonly Permission CloseIssues =
            new Permission("CloseIssues", "Close issues");

        public static readonly Permission OpenIssues =
            new Permission("OpenIssues", "Re-open issues");
        
        public static readonly Permission HideIssues =
            new Permission("HideIssues", "Hide issues");

        public static readonly Permission ShowIssues =
            new Permission("ShowIssues", "Unhide issues");

        public static readonly Permission ViewHiddenIssues =
            new Permission("ViewHiddenIssues", "View hidden issues");

        public static readonly Permission ViewPrivateIssues =
            new Permission("ViewPrivateIssues", "View private issues");

        public static readonly Permission HideIssueComments =
            new Permission("HideIssueComments", "Hide comments");

        public static readonly Permission ShowIssueComments =
            new Permission("ShowIssueComments", "Unhide comments");
        
        public static readonly Permission ViewHiddenIssueComments =
            new Permission("ViewHiddenIssueComments", "View hidden comments");

        public static readonly Permission IssueToSpam =
            new Permission("IssueToSpam", "Move issues to SPAM");

        public static readonly Permission IssueFromSpam =
            new Permission("IssueFromSpam", "Remove issues from SPAM");

        public static readonly Permission ViewSpamIssues =
            new Permission("ViewSpamIssues", "View issues flagged as SPAM");

        public static readonly Permission IssueCommentToSpam =
            new Permission("IssueCommentToSpam", "Move comments to SPAM");

        public static readonly Permission IssueCommentFromSpam =
            new Permission("IssueCommentFromSpam", "Remove comments from SPAM");

        public static readonly Permission ViewSpamIssueComments =
            new Permission("ViewSpamIssueComments", "View comments flagged as SPAM");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostIssues,
                PostIssueComments,
                EditOwnIssues,
                EditAnyIssue,
                EditOwnIssueComments,
                EditAnyIssueComment,
                DeleteOwnIssues,
                RestoreOwnIssues,
                PermanentDeleteOwnIssues,
                DeleteAnyIssue,
                RestoreAnyIssue,
                PermanentDeleteAnyIssue,
                ViewDeletedIssues,
                DeleteOwnIssueComments,
                RestoreOwnIssueComments,
                PermanentDeleteOwnIssueComments,
                DeleteAnyIssueComment,
                RestoreAnyIssueComment,
                PermanentDeleteAnyIssueComment,
                ViewDeletedIssueComments,
                ReportIssues,
                ReportIssueComments,
                PinIssues,
                UnpinIssues,
                LockIssues,
                UnlockIssues,
                CloseIssues,
                OpenIssues,
                HideIssues,
                ShowIssues,
                ViewHiddenIssues,
                ViewPrivateIssues,
                HideIssueComments,
                ShowIssueComments,
                ViewHiddenIssueComments,
                IssueToSpam,
                IssueFromSpam,
                ViewSpamIssues,
                IssueCommentToSpam,
                IssueCommentFromSpam,
                ViewSpamIssueComments
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
                        PostIssues,
                        PostIssueComments,
                        EditOwnIssues,
                        EditAnyIssue,
                        EditOwnIssueComments,
                        EditAnyIssueComment,
                        DeleteOwnIssues,
                        RestoreOwnIssues,
                        PermanentDeleteOwnIssues,
                        DeleteAnyIssue,
                        RestoreAnyIssue,
                        PermanentDeleteAnyIssue,
                        ViewDeletedIssues,
                        DeleteOwnIssueComments,
                        RestoreOwnIssueComments,
                        PermanentDeleteOwnIssueComments,
                        DeleteAnyIssueComment,
                        RestoreAnyIssueComment,
                        PermanentDeleteAnyIssueComment,
                        ViewDeletedIssueComments,
                        ReportIssues,
                        ReportIssueComments,
                        PinIssues,
                        UnpinIssues,
                        LockIssues,
                        UnlockIssues,
                        CloseIssues,
                        OpenIssues,
                        HideIssues,
                        ShowIssues,
                        ViewHiddenIssues,
                        ViewPrivateIssues,
                        HideIssueComments,
                        ShowIssueComments,
                        ViewHiddenIssueComments,
                        IssueToSpam,
                        IssueFromSpam,
                        ViewSpamIssues,
                        IssueCommentToSpam,
                        IssueCommentFromSpam,
                        ViewSpamIssueComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostIssues,
                        EditOwnIssues,
                        DeleteOwnIssues,
                        PostIssueComments,
                        EditOwnIssueComments,
                        DeleteOwnIssueComments,
                        ReportIssues,
                        ReportIssueComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostIssues,
                        PostIssueComments,
                        EditOwnIssues,
                        EditOwnIssueComments,
                        DeleteOwnIssues,
                        RestoreOwnIssues,
                        PermanentDeleteOwnIssues,
                        ViewDeletedIssues,
                        DeleteOwnIssueComments,
                        RestoreOwnIssueComments,
                        PermanentDeleteOwnIssueComments,
                        ViewDeletedIssueComments,
                        ReportIssues,
                        ReportIssueComments,
                        PinIssues,
                        UnpinIssues,
                        LockIssues,
                        UnlockIssues,
                        CloseIssues,
                        OpenIssues,
                        HideIssues,
                        ShowIssues,
                        ViewHiddenIssues,
                        ViewPrivateIssues,
                        HideIssueComments,
                        ShowIssueComments,
                        ViewHiddenIssueComments,
                        IssueToSpam,
                        IssueFromSpam,
                        ViewSpamIssues,
                        IssueCommentToSpam,
                        IssueCommentFromSpam,
                        ViewSpamIssueComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportIssues,
                        ReportIssueComments
                    }
                }
            };
        }

    }

}
