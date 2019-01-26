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
            new ModeratorPermission("EditTopics", "Can edit topics");

        public static readonly ModeratorPermission EditReplies =
            new ModeratorPermission("EditReplies", "Can edit replies");

        public static readonly ModeratorPermission DeleteTopics =
            new ModeratorPermission("DeleteTopics", "Can delete topics");

        public static readonly ModeratorPermission DeleteReplies =
            new ModeratorPermission("DeleteReplies", "Can delete replies");

        public static readonly ModeratorPermission PinTopics =
            new ModeratorPermission("PinTopics", "Can pin topics");

        public static readonly ModeratorPermission UnpinTopics =
            new ModeratorPermission("UnpinTopics", "Can unpin topics");
        
        public static readonly ModeratorPermission HideTopics =
            new ModeratorPermission("HideTopics", "Can hide topics");

        public static readonly ModeratorPermission ShowTopics =
            new ModeratorPermission("ShowTopics", "Can make topics visible");

        public static readonly ModeratorPermission HideReplies =
            new ModeratorPermission("HideReplies", "Can hide replies");

        public static readonly ModeratorPermission ShowReplies =
            new ModeratorPermission("ShowReplies", "Can make replies visible");
        
        public static readonly ModeratorPermission TopicsToSpam =
            new ModeratorPermission("TopicsToSpam", "Can move topics to SPAM");

        public static readonly ModeratorPermission TopicsFromSpam =
            new ModeratorPermission("TopicsFromSpam", "Can remove topics from SPAM");

        public static readonly ModeratorPermission RepliesToSpam =
            new ModeratorPermission("RepliesToSpam", "Can move replies to SPAM");

        public static readonly ModeratorPermission RepliesFromSpam =
            new ModeratorPermission("RepliesFromSpam", "Can remove replies from SPAM");
        
        public IEnumerable<ModeratorPermission> GetPermissions()
        {
            return new[]
            {
                EditTopics,
                EditReplies,
                DeleteTopics,
                DeleteReplies,
                PinTopics,
                UnpinTopics,
                HideTopics,
                ShowTopics,
                HideReplies,
                ShowReplies,
                TopicsToSpam,
                TopicsFromSpam,
                RepliesToSpam,
                RepliesFromSpam
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
                        HideTopics,
                        ShowTopics,
                        HideReplies,
                        ShowReplies,
                        TopicsToSpam,
                        TopicsFromSpam,
                        RepliesToSpam,
                        RepliesFromSpam
                    }
                }
            };
        }
    }

}
