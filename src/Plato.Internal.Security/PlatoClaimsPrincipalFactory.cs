using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Security
{

    // <summary>
    // A custom UserClaimsPrincipalFactory implementation.
    // Roles within Plato can contain many claims. For this reason to avoid cookie
    // chunking and exceeding maximum request header length issues caused by persisting claims
    // within a cookie we don't persist the role claims within the cookie and instead
    // query these claims as necessary based on our minimal claims principal created by this implementation.
    // </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    public class PlatoClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser>
        where TUser : class, IUser
        where TRole : class
    {

        public PlatoClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) :
            base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            
            // Our list of claims
            var claims = new List<Claim>();

            // Get user details
            var userId = await this.UserManager.GetUserIdAsync(user);
            var userNameAsync = await this.UserManager.GetUserNameAsync(user);

            // Create user detail claims
            claims.Add(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            claims.Add(new Claim(Options.ClaimsIdentity.UserNameClaimType, userNameAsync));

            // If the security stamp changes the authentication cookie will be invalidated
            if (this.UserManager.SupportsUserSecurityStamp)
            {
                claims.Add(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, await this.UserManager.GetSecurityStampAsync(user)));
            }

            // User claims
            if (this.UserManager.SupportsUserClaim)
            {
                claims.AddRange((IEnumerable<Claim>)await this.UserManager.GetClaimsAsync(user));
            }
     
            // NOTE: We don't store role claims here

            // Return identity
            return new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            
        }



    }
}