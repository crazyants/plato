using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Questions.Categories.Follow
{
    public class Permissions : IPermissionsProvider<Permission>
    {

        public static readonly Permission FollowQuestionCategories =
            new Permission("FollowQuestionCategories", "Can follow all categories");

        public static readonly Permission FollowQuestionCategory =
            new Permission("FollowQuestionCategory", "Can follow any individual category");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                FollowQuestionCategories,
                FollowQuestionCategory
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
                        FollowQuestionCategories,
                        FollowQuestionCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Member,
                    Permissions = new[]
                    {
                        FollowQuestionCategories,
                        FollowQuestionCategory
                    }
                },
                new DefaultPermissions<Permission>
                {
                    RoleName = DefaultRoles.Staff,
                    Permissions = new[]
                    {
                        FollowQuestionCategories,
                        FollowQuestionCategory
                    }
                }
            };

        }

    }

}
