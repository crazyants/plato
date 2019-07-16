using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowQuestions =
            new Permission("FollowQuestions", "Can follow questions");

        public static readonly Permission FollowNewQuestions =
            new Permission("FollowNewQuestions", "Automatically follow new questions");

        public static readonly Permission FollowParticipatedQuestions =
            new Permission("FollowParticipatedQuestions", "Automatically follow questions when posting replies");


        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowQuestions,
                FollowNewQuestions,
                FollowParticipatedQuestions
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
                        FollowQuestions,
                        FollowNewQuestions,
                        FollowParticipatedQuestions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowQuestions,
                        FollowNewQuestions,
                        FollowParticipatedQuestions
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowQuestions,
                        FollowNewQuestions,
                        FollowParticipatedQuestions
                    }
                }
            };

        }

    }

}
