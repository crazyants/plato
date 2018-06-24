using System.Collections.Generic;
using Plato.Internal.Security.Abstractions;

namespace Plato.Users.Social
{
    public class PermissionsProvider : IPermissionsProvider
    {
        public static readonly Permission ContentPreview = 
            new Permission("Demo", "Display the demo feature");

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] { ContentPreview };
        }

        public IEnumerable<DefaultPermissions> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissions
                {
                    Name = "Administrator",
                    Permissions = new[] { ContentPreview }
                }
            };
        }
    }

}
