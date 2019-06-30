using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;
using Plato.Moderation.Models;

namespace Plato.Discuss.Categories.Moderators
{
    public class ModeratorPermissions : IPermissionsProvider<ModeratorPermission>
    {
        
        public static readonly ModeratorPermission EditTopics =
            new ModeratorPermission("EditTopics", "Edit topics", new List<IPermission>()
            {
                Discuss.Permissions.EditOwnTopics,
                Discuss.Permissions.EditAnyTopic,
            });

        public static readonly ModeratorPermission EditReplies =
            new ModeratorPermission("EditReplies", "Edit replies", new List<IPermission>()
            {
                Discuss.Permissions.EditOwnReplies,
                Discuss.Permissions.EditAnyReply,
            });

        public static readonly ModeratorPermission DeleteTopics =
            new ModeratorPermission("DeleteTopics", "Delete topics", new List<IPermission>()
            {
                Discuss.Permissions.DeleteOwnReplies,
                Discuss.Permissions.DeleteAnyReply,
            });

        public static readonly ModeratorPermission RestoreTopics =
            new ModeratorPermission("RestoreTopics", "Restore deleted topics", new List<IPermission>()
            {
                Discuss.Permissions.RestoreOwnTopics,
                Discuss.Permissions.RestoreAnyTopic,
            });
        
        public static readonly ModeratorPermission DeleteReplies =
            new ModeratorPermission("DeleteReplies", "Delete replies", new List<IPermission>()
            {
                Discuss.Permissions.DeleteOwnReplies,
                Discuss.Permissions.DeleteAnyReply,
            });

        public static readonly ModeratorPermission RestoreReplies =
            new ModeratorPermission("RestoreReply", "Restore deleted replies", new List<IPermission>()
            {
                Discuss.Permissions.RestoreOwnReplies,
                Discuss.Permissions.RestoreAnyTopic,
            });
        
        public static readonly ModeratorPermission PinTopics =
            new ModeratorPermission("PinTopics", "Pin topics", new List<IPermission>()
            {
                Discuss.Permissions.PinTopics
            });

        public static readonly ModeratorPermission UnpinTopics =
            new ModeratorPermission("UnpinTopics", "Unpin topics", new List<IPermission>()
            {
                Discuss.Permissions.UnpinTopics
            });
        
        public static readonly ModeratorPermission LockTopics =
            new ModeratorPermission("LockTopics", "Lock topics", new List<IPermission>()
            {
                Discuss.Permissions.LockTopics
            });

        public static readonly ModeratorPermission UnlockTopics =
            new ModeratorPermission("UnlockTopics", "Unlock topics", new List<IPermission>()
            {
                Discuss.Permissions.UnlockTopics
            });
        
        public static readonly ModeratorPermission HideTopics =
            new ModeratorPermission("HideTopics", "Hide topics", new List<IPermission>()
            {
                Discuss.Permissions.HideTopics
            });
        
        public static readonly ModeratorPermission ShowTopics =
            new ModeratorPermission("ShowTopics", "Show topics", new List<IPermission>()
            {
                Discuss.Permissions.ShowTopics
            });

        public static readonly ModeratorPermission HideReplies =
            new ModeratorPermission("HideReplies", "Hide replies", new List<IPermission>()
            {
                Discuss.Permissions.HideReplies
            });

        public static readonly ModeratorPermission ShowReplies =
            new ModeratorPermission("ShowReplies", "Show replies", new List<IPermission>()
            {
                Discuss.Permissions.ShowReplies
            });
        
        public static readonly ModeratorPermission TopicToSpam =
            new ModeratorPermission("TopicsToSpam", "Move topics to SPAM", new List<IPermission>()
            {
                Discuss.Permissions.TopicToSpam
            });

        public static readonly ModeratorPermission TopicFromSpam =
            new ModeratorPermission("TopicsFromSpam", "Remove topics from SPAM", new List<IPermission>()
            {
                Discuss.Permissions.TopicFromSpam
            });

        public static readonly ModeratorPermission ReplyToSpam =
            new ModeratorPermission("RepliesToSpam", "Move replies to SPAM", new List<IPermission>()
            {
                Discuss.Permissions.ReplyToSpam
            });

        public static readonly ModeratorPermission ReplyFromSpam =
            new ModeratorPermission("RepliesFromSpam", "Remove replies from SPAM", new List<IPermission>()
            {
                Discuss.Permissions.ReplyFromSpam
            });
        
        public IEnumerable<ModeratorPermission> GetPermissions()
        {
            return new[]
            {
                EditTopics,
                EditReplies,
                DeleteTopics,
                RestoreTopics,
                DeleteReplies,
                RestoreReplies,
                PinTopics,
                UnpinTopics,
                LockTopics,
                UnlockTopics,
                HideTopics,
                ShowTopics,
                HideReplies,
                ShowReplies,
                TopicToSpam,
                TopicFromSpam,
                ReplyToSpam,
                ReplyFromSpam
            };
        }

        public IEnumerable<DefaultPermissions<ModeratorPermission>> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions<ModeratorPermission>
                {
                    RoleName = DefaultRoles.Administrator,
                    Permissions = new[]
                    {
                        EditTopics,
                        EditReplies,
                        DeleteTopics,
                        DeleteReplies,
                        PinTopics,
                        UnpinTopics,
                        LockTopics,
                        UnlockTopics,
                        HideTopics,
                        ShowTopics,
                        HideReplies,
                        ShowReplies,
                        TopicToSpam,
                        TopicFromSpam,
                        ReplyToSpam,
                        ReplyFromSpam
                    }
                }
            };
        }
    }

}
