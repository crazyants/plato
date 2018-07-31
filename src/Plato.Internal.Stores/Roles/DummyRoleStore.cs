using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Models.Roles;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Internal.Stores.Roles
{

    public class DummyRoleStore :
        IRoleStore<Role>,
        IRoleClaimStore<Role>
    {

        #region "IRoleStore"

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Failed());
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Failed());
        }

        public void Dispose()
        {
        }

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(Role));
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(Role));
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(string));
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(string));
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(string));
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Failed());
        }

        #endregion

        #region "IRoleClaimStore"

        public Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default(CancellationToken))
        {
            return null;
        }

        public Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        #endregion

    }

}
