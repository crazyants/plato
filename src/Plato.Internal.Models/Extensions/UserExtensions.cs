using System;
using System.Linq;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Models.Extensions
{
    public static class UserExtensions
    {

        public static bool IsInRole(this IUser user, string roleName)
        {

            if (user.RoleNames == null)
            {
                return false;
            }

            if (!user.RoleNames.Any())
            {
                return false;
            }

            foreach (var name in user.RoleNames)
            {
                if (name.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;

        }

    }

}
