using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Tags
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostTopicTags =
            new Permission("PostTopicTags", "Can add tags when posting topics");

        public static readonly Permission EditTopicTags =
            new Permission("EditTopicTags", "Can edit tags when editing topics");

        public static readonly Permission PostReplyTags =
            new Permission("PostReplyTags", "Can add tags when posting replies");

        public static readonly Permission EditReplyTags =
            new Permission("EditReplyTags", "Can edit tags when editing replies");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostTopicTags,
                EditTopicTags,
                PostReplyTags,
                EditReplyTags
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
                        PostTopicTags,
                        EditTopicTags,
                        PostReplyTags,
                        EditReplyTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostTopicTags,
                        EditTopicTags,
                        PostReplyTags,
                        EditReplyTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostTopicTags,
                        EditTopicTags,
                        PostReplyTags,
                        EditReplyTags
                    }
                }
            };

        }

    }

}
