using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.Tags
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostDocTags =
            new Permission("PostDocTags", "Can add tags when posting docs");

        public static readonly Permission EditDocTags =
            new Permission("EditDocTags", "Can edit tags when editing docs");

        public static readonly Permission PostDocCommentTags =
            new Permission("PostDocCommentTags", "Can add tags when posting doc comments");

        public static readonly Permission EditDocCommentTags =
            new Permission("EditDocCommentTags", "Can edit tags when editing doc comments");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostDocTags,
                EditDocTags,
                PostDocCommentTags,
                EditDocCommentTags
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
                        PostDocTags,
                        EditDocTags,
                        PostDocCommentTags,
                        EditDocCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostDocTags,
                        EditDocTags,
                        PostDocCommentTags,
                        EditDocCommentTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostDocTags,
                        EditDocTags,
                        PostDocCommentTags,
                        EditDocCommentTags
                    }
                }
            };

        }

    }

}
