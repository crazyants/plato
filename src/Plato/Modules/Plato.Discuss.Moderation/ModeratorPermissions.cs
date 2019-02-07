using System;
using System.Collections.Generic;
using System.Text;
using Plato.Internal.Security.Abstractions;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation
{
    public class ModeratorPermissions : IPermissionsProvider<ModeratorPermission>
    {

        public static readonly ModeratorPermission EditTopics =
            new ModeratorPermission("EditTopics", "Edit topics");

        public static readonly ModeratorPermission EditReplies =
            new ModeratorPermission("EditReplies", "Edit replies");

        public static readonly ModeratorPermission DeleteTopics =
            new ModeratorPermission("DeleteTopics", "Delete topics");

        public static readonly ModeratorPermission RestoreTopics =
            new ModeratorPermission("RestoreTopics", "Restore deleted topics");
        
        public static readonly ModeratorPermission DeleteReplies =
            new ModeratorPermission("DeleteReplies", "Delete replies");

        public static readonly ModeratorPermission RestoreReplies =
            new ModeratorPermission("RestoreReply", "Restore deleted replies");
        
        public static readonly ModeratorPermission PinTopics =
            new ModeratorPermission("PinTopics", "Pin topics");

        public static readonly ModeratorPermission UnpinTopics =
            new ModeratorPermission("UnpinTopics", "Unpin topics");
        
        public static readonly ModeratorPermission CloseTopics =
            new ModeratorPermission("CloseTopics", "Lock topics");

        public static readonly ModeratorPermission OpenTopics =
            new ModeratorPermission("OpenTopics", "Unlock topics");

        public static readonly ModeratorPermission HideTopics =
            new ModeratorPermission("HideTopics", "Hide topics");
        
        public static readonly ModeratorPermission ShowTopics =
            new ModeratorPermission("ShowTopics", "Show topics");

        public static readonly ModeratorPermission HideReplies =
            new ModeratorPermission("HideReplies", "Hide replies");

        public static readonly ModeratorPermission ShowReplies =
            new ModeratorPermission("ShowReplies", "Show replies");
        
        public static readonly ModeratorPermission TopicToSpam =
            new ModeratorPermission("TopicsToSpam", "Move topics to SPAM");

        public static readonly ModeratorPermission TopicFromSpam =
            new ModeratorPermission("TopicsFromSpam", "Remove topics from SPAM");

        public static readonly ModeratorPermission ReplyToSpam =
            new ModeratorPermission("RepliesToSpam", "Move replies to SPAM");

        public static readonly ModeratorPermission ReplyFromSpam =
            new ModeratorPermission("RepliesFromSpam", "Remove replies from SPAM");
        
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
                CloseTopics,
                OpenTopics,
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
                        CloseTopics,
                        OpenTopics,
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
