using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Answers
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission MarkOwnRepliesAnswer =
            new Permission("MarkOwnRepliesAnswer", "Mark answers to own questions as accepted answer");

        public static readonly Permission MarkAnyReplyAnswer =
            new Permission("MarkAnyReplyAnswer", "Mark answers to any question as accepted answer");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                MarkOwnRepliesAnswer,
                MarkAnyReplyAnswer
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
                        MarkOwnRepliesAnswer,
                        MarkAnyReplyAnswer
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        MarkOwnRepliesAnswer
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        MarkOwnRepliesAnswer,
                        MarkAnyReplyAnswer
                    }
                }
            };
        }

    }

}
