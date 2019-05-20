using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Ideas.Tags
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostIdeaTags =
            new Permission("PostIdeaTags", "Can add tags when posting ideas");

        public static readonly Permission EditIdeaTags =
            new Permission("EditIdeaTags", "Can edit tags when editing ideas");

        public static readonly Permission PostIdeaCommentTags =
            new Permission("PostIdeaCommentTags", "Can add tags when posting idea comments");

        public static readonly Permission EditIdeaCommentTags =
            new Permission("EditIdeaCommentTags", "Can edit tags when editing idea comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostIdeaTags,
                EditIdeaTags,
                PostIdeaCommentTags,
                EditIdeaCommentTags
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
                        PostIdeaTags,
                        EditIdeaTags,
                        PostIdeaCommentTags,
                        EditIdeaCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostIdeaTags,
                        EditIdeaTags,
                        PostIdeaCommentTags,
                        EditIdeaCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostIdeaTags,
                        EditIdeaTags,
                        PostIdeaCommentTags,
                        EditIdeaCommentTags
                    }
                }
            };

        }

    }

}
