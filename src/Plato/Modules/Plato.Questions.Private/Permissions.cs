using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Private
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission QuestionsPrivateCreatePublic =
            new Permission("QuestionsPrivateCreatePublic", "Post public questions");

        public static readonly Permission QuestionsPrivateCreatePrivate =
            new Permission("QuestionsPrivateCreatePrivate", "Post private questions");

        public static readonly Permission QuestionsPrivateToPublic =
            new Permission("QuestionsPrivateToPublic", "Convert questions to public");

        public static readonly Permission QuestionsPrivateToPrivate =
            new Permission("QuestionsPrivateToPrivate", "Convert questions to private");
        
        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                QuestionsPrivateCreatePublic,
                QuestionsPrivateCreatePrivate,
                QuestionsPrivateToPublic,
                QuestionsPrivateToPrivate
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
                        QuestionsPrivateCreatePublic,
                        QuestionsPrivateCreatePrivate,
                        QuestionsPrivateToPublic,
                        QuestionsPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        QuestionsPrivateCreatePublic,
                        QuestionsPrivateCreatePrivate,
                        QuestionsPrivateToPublic,
                        QuestionsPrivateToPrivate
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        QuestionsPrivateCreatePublic,
                        QuestionsPrivateCreatePrivate,
                        QuestionsPrivateToPublic,
                        QuestionsPrivateToPrivate
                    }
                }
            };

        }

    }

}
