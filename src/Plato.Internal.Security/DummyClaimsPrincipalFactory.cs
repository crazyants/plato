using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Security
{
    
    public class DummyClaimsPrincipalFactory<TUser> : IDummyClaimsPrincipalFactory<TUser> where TUser : class, IUser
    {
        
        private IdentityOptions Options { get; }

        public DummyClaimsPrincipalFactory(IOptions<IdentityOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }
        
        public Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            // Our list of claims
            var claims = new List<Claim>();

            if (user.Id > 0)
            {
                claims.Add(new Claim(Options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()));
            }

            if (!string.IsNullOrEmpty(user.UserName))
            {
                claims.Add(new Claim(Options.ClaimsIdentity.UserNameClaimType, user.UserName));
            }
            
            if (user.RoleNames != null)
            {
                foreach (var roleName in user.RoleNames)
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
            }
          
            // Build identity
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

            // Return principal
            return Task.FromResult(new ClaimsPrincipal(identity));

        }
    }
}
