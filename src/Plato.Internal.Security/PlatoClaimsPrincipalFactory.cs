using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Security
{
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

         
            var claims = new List<Claim>();
            var userId = await _userManager.GetUserIdAsync(user);
            var userNameAsync = await _userManager.GetUserNameAsync(user);

            claims.Add(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            claims.Add(new Claim(Options.ClaimsIdentity.UserNameClaimType, userNameAsync));

            if (this.UserManager.SupportsUserSecurityStamp)
            {
                claims.Add(new Claim(Options.ClaimsIdentity.SecurityStampClaimType, await this.UserManager.GetSecurityStampAsync(user)));
            }

            if (this.UserManager.SupportsUserClaim)
            {
                claims.AddRange((IEnumerable<Claim>)await this.UserManager.GetClaimsAsync(user));
            }
     
            foreach (string roleName in (IEnumerable<string>)await _userManager.GetRolesAsync(user))
            {

                claims.Add(new Claim(Options.ClaimsIdentity.RoleClaimType, roleName));
                
                // Roles within Plato can contain many claims. For this reason to avoid cookie
                // chunking and exceeding maximum request header length limitations we don't store the role
                // claims within a client side cookie and instead query these as necessary based on the role claims

                //var byNameAsync = await _roleManager.FindByNameAsync(roleName);
                //if (byNameAsync != null)
                //{
                //    ClaimsIdentity claimsIdentity = id;
                //    claimsIdentity.AddClaims((IEnumerable<Claim>)await _roleManager.GetClaimsAsync(byNameAsync));
                //    claimsIdentity = (ClaimsIdentity)null;
                //}
             
            }

            return new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            
        }

    }
}