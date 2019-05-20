using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Tags
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission PostQuestionTags =
            new Permission("PostQuestionTags", "Can add tags when posting questions");

        public static readonly Permission EditQuestionTags =
            new Permission("EditQuestionTags", "Can edit tags when editing questions");

        public static readonly Permission PostQuestionAnswerTags =
            new Permission("PostQuestionAnswerTags", "Can add tags when posting question answers");

        public static readonly Permission EditQuestionAnswerTags =
            new Permission("EditQuestionAnswerTags", "Can edit tags when editing question answers");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                PostQuestionTags,
                EditQuestionTags,
                PostQuestionAnswerTags,
                EditQuestionAnswerTags
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
                        PostQuestionTags,
                        EditQuestionTags,
                        PostQuestionAnswerTags,
                        EditQuestionAnswerTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        PostQuestionTags,
                        EditQuestionTags,
                        PostQuestionAnswerTags,
                        EditQuestionAnswerTags
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        PostQuestionTags,
                        EditQuestionTags,
                        PostQuestionAnswerTags,
                        EditQuestionAnswerTags
                    }
                }
            };

        }

    }

}
