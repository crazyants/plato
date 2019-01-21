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
            new ModeratorPermission("EditAnyTopic", "Can edit any topic");

        public static readonly ModeratorPermission EditReplies =
            new ModeratorPermission("EditAnyReply", "Can edit any reply");

        public static readonly ModeratorPermission DeleteTopics =
            new ModeratorPermission("DeleteAnyTopic", "Can delete any topic");

        public static readonly ModeratorPermission DeleteReplies =
            new ModeratorPermission("DeleteAnyReply", "Can delete any reply");

        public static readonly ModeratorPermission PinTopics =
            new ModeratorPermission("PinTopics", "Can pin any topic");

        public IEnumerable<ModeratorPermission> GetPermissions()
        {
            return new[]
            {
                EditTopics,
                EditReplies,
                DeleteTopics,
                DeleteReplies,
                PinTopics
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
                        PinTopics
                    }
                }
            };
        }
    }

}
