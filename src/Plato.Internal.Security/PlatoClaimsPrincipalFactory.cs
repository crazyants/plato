using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Security
{

    // <summary>
    // A custom UserClaimsPrincipalFactory implementation.
    // Roles within Plato can contain many claims. For this reason to avoid cookie
    // chunking and exceeding maximum request header length issues caused by persisting claims
    // within the a cookie we don't persist the role claims within the cookie and instead
    // query these claims as necessary based on our minimal claims principal created by this implementation.
    // </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    public class PlatoClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser>
        where TUser : class
        where TRole : class
    {

        private readonly UserManager<TUser> _userManager;
        private readonly RoleManager<TRole> _roleManager;

        public PlatoClaimsPrincipalFactory(
            UserManager<TUser> userManager,
            IOptions<IdentityOptions> optionsAccessor, 
            RoleManager<TRole> roleManager) :
            base(userManager, optionsAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(TUser user)
        {
            
            // Our list of claims
            var claims = new List<Claim>();

            // Get user details
            var userId = await _userManager.GetUserIdAsync(user);
            var userNameAsync = await _userManager.GetUserNameAsync(user);

            // Create user detail claims
            claims.Add(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            claims.Add(new Claim(Options.ClaimsIdentity.UserNameClaimType, userNameAsync));

            // If the security stamp changes the authentication cookie will be ivalidated
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