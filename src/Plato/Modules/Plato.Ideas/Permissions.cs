using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostIdeas =
            new Permission("PostIdeas", "Post ideas");

        public static readonly Permission PostIdeaComments =
            new Permission("PostIdeaComments", "Post idea comments");

        public static readonly Permission EditOwnIdeas =
            new Permission("EditOwnIdeas", "Edit own ideas");

        public static readonly Permission EditAnyIdea =
            new Permission("EditAnyIdea", "Edit any idea");
        
        public static readonly Permission EditOwnIdeaComments =
            new Permission("EditOwnIdeaComments", "Edit own idea comments");

        public static readonly Permission EditAnyIdeaComment =
            new Permission("EditAnyIdeaComment", "Edit any idea comment");
        
        public static readonly Permission DeleteOwnIdeas = 
            new Permission("DeleteOwnIdeas", "Delete own ideas");

        public static readonly Permission RestoreOwnIdeas =
            new Permission("RestoreOwnIdeas", "Restore own ideas");
        
        public static readonly Permission DeleteAnyIdea =
            new Permission("DeleteAnyIdea", "Delete any idea");

        public static readonly Permission RestoreAnyIdea =
            new Permission("RestoreAnyIdea", "Restore any idea");

        public static readonly Permission ViewDeletedIdeas =
            new Permission("ViewDeletedIdeas", "View deleted ideas");
        
        public static readonly Permission DeleteOwnIdeaComments =
            new Permission("DeleteOwnIdeaComments", "Delete own idea comments");

        public static readonly Permission RestoreOwnIdeaComments =
            new Permission("RestoreOwnIdeaComments", "Restore own idea comments");
        
        public static readonly Permission DeleteAnyIdeaComment =
            new Permission("DeleteAnyIdeaComment", "Delete any idea comment");

        public static readonly Permission RestoreAnyIdeaComment =
            new Permission("RestoreAnyIdeaComment", "Restore any idea comment");

        public static readonly Permission ViewDeletedIdeaComment =
            new Permission("ViewDeletedIdeaComment", "View deleted idea comments");

        public static readonly Permission ReportIdeas =
            new Permission("ReportIdeas", "Report ideas");

        public static readonly Permission ReportIdeaComments =
            new Permission("ReportIdeaComments", "Report idea comments");
        
        public static readonly Permission PinIdeas =
            new Permission("PinIdeas", "Pin ideas");

        public static readonly Permission UnpinIdeas =
            new Permission("UnpinIdeas", "Unpin ideas");

        public static readonly Permission LockIdeas =
            new Permission("LockIdeas", "Lock ideas");

        public static readonly Permission UnlockIdeas =
            new Permission("UnlockIdeas", "Unlock ideas");

        public static readonly Permission HideIdeas =
            new Permission("HideIdeas", "Hide ideas");

        public static readonly Permission ShowIdeas =
            new Permission("ShowIdeas", "Unhide ideas");

        public static readonly Permission ViewPrivateIdeas =
            new Permission("ViewPrivateIdeas", "View hidden ideas");

        public static readonly Permission HideIdeaComments =
            new Permission("HideIdeaComments", "Hide comments");

        public static readonly Permission ShowIdeaComments =
            new Permission("ShowIdeaComments", "Unhide comments");

        public static readonly Permission ViewPrivateIdeaComments =
            new Permission("ViewPrivateIdeaComments", "View hidden comments");

        public static readonly Permission IdeaToSpam =
            new Permission("IdeaToSpam", "Move ideas to SPAM");

        public static readonly Permission IdeaFromSpam =
            new Permission("IdeaFromSpam", "Remove ideas from SPAM");

        public static readonly Permission ViewSpamIdeas =
            new Permission("ViewSpamIdeas", "View ideas flagged as SPAM");

        public static readonly Permission IdeaCommentToSpam =
            new Permission("IdeaCommentToSpam", "Move comments to SPAM");

        public static readonly Permission IdeaCommentFromSpam =
            new Permission("IdeaCommentFromSpam", "Remove comments from SPAM");

        public static readonly Permission ViewSpamIdeaComments =
            new Permission("ViewSpamIdeaComments", "View comments flagged as SPAM");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostIdeas,
                PostIdeaComments,
                EditOwnIdeas,
                EditAnyIdea,
                EditOwnIdeaComments,
                EditAnyIdeaComment,
                DeleteOwnIdeas,
                RestoreOwnIdeas,
                DeleteAnyIdea,
                RestoreAnyIdea,
                ViewDeletedIdeas,
                DeleteOwnIdeaComments,
                RestoreOwnIdeaComments,
                DeleteAnyIdeaComment,
                RestoreAnyIdeaComment,
                ViewDeletedIdeaComment,
                ReportIdeas,
                ReportIdeaComments,
                PinIdeas,
                UnpinIdeas,
                LockIdeas,
                UnlockIdeas,
                HideIdeas,
                ShowIdeas,
                ViewPrivateIdeas,
                HideIdeaComments,
                ShowIdeaComments,
                ViewPrivateIdeaComments,
                IdeaToSpam,
                IdeaFromSpam,
                ViewSpamIdeas,
                IdeaCommentToSpam,
                IdeaCommentFromSpam,
                ViewSpamIdeaComments
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
                        PostIdeas,
                        PostIdeaComments,
                        EditOwnIdeas,
                        EditAnyIdea,
                        EditOwnIdeaComments,
                        EditAnyIdeaComment,
                        DeleteOwnIdeas,
                        RestoreOwnIdeas,
                        DeleteAnyIdea,
                        RestoreAnyIdea,
                        ViewDeletedIdeas,
                        DeleteOwnIdeaComments,
                        RestoreOwnIdeaComments,
                        DeleteAnyIdeaComment,
                        RestoreAnyIdeaComment,
                        ViewDeletedIdeaComment,
                        ReportIdeas,
                        ReportIdeaComments,
                        PinIdeas,
                        UnpinIdeas,
                        LockIdeas,
                        UnlockIdeas,
                        HideIdeas,
                        ShowIdeas,
                        ViewPrivateIdeas,
                        HideIdeaComments,
                        ShowIdeaComments,
                        ViewPrivateIdeaComments,
                        IdeaToSpam,
                        IdeaFromSpam,
                        ViewSpamIdeas,
                        IdeaCommentToSpam,
                        IdeaCommentFromSpam,
                        ViewSpamIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostIdeas,
                        PostIdeaComments,
                        EditOwnIdeas,
                        EditOwnIdeaComments,
                        DeleteOwnIdeaComments,
                        ReportIdeas,
                        ReportIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostIdeas,
                        PostIdeaComments,
                        EditOwnIdeas,
                        EditAnyIdea,
                        EditOwnIdeaComments,
                        EditAnyIdeaComment,
                        DeleteOwnIdeas,
                        RestoreOwnIdeas,
                        DeleteAnyIdea,
                        RestoreAnyIdea,
                        ViewDeletedIdeas,
                        DeleteOwnIdeaComments,
                        RestoreOwnIdeaComments,
                        DeleteAnyIdeaComment,
                        RestoreAnyIdeaComment,
                        ViewDeletedIdeaComment,
                        ReportIdeas,
                        ReportIdeaComments,
                        PinIdeas,
                        UnpinIdeas,
                        LockIdeas,
                        UnlockIdeas,
                        HideIdeas,
                        ShowIdeas,
                        ViewPrivateIdeas,
                        HideIdeaComments,
                        ShowIdeaComments,
                        ViewPrivateIdeaComments,
                        IdeaToSpam,
                        IdeaFromSpam,
                        ViewSpamIdeas,
                        IdeaCommentToSpam,
                        IdeaCommentFromSpam,
                        ViewSpamIdeaComments
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Anonymous,
                    Permissions = new[]
                    {
                        ReportIdeas,
                        ReportIdeaComments
                    }
                }
            };
        }

    }

}
