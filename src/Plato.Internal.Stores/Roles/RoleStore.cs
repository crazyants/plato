using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Plato.Internal.Models.Roles;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Internal.Stores.Roles
{
    public class RoleStore :
        IRoleStore<Role>, 
        IRoleClaimStore<Role>
    {

        private readonly IPlatoRoleStore _platoRoleStore;

        public RoleStore(IPlatoRoleStore platoRoleStore)
        {
            _platoRoleStore = platoRoleStore;
        }

        #region "IRoleStore"

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var newRole = await _platoRoleStore.CreateAsync(role);
            if ((newRole != null) && (newRole.Id > 0))
                return IdentityResult.Success;

            return IdentityResult.Failed();
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));
            
            if (await _platoRoleStore.DeleteAsync(role))
                return IdentityResult.Success;

            return IdentityResult.Failed();
        }

        public void Dispose()
        {
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (!int.TryParse(roleId, out var id))
                throw new ArgumentException("roleId must be of type int");
            return await _platoRoleStore.GetByIdAsync(id);
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return await _platoRoleStore.GetByNormalizedNameAsync(normalizedRoleName);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.NormalizedName = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var updatedRole = await _platoRoleStore.CreateAsync(role);
            if ((updatedRole != null) && (updatedRole.Id > 0))
                return IdentityResult.Success;

            return IdentityResult.Failed();
        }

        #endregion

        #region "IRoleClaimStore"

        public Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            ((Role)role).RoleClaims.Add(new RoleClaim { ClaimType = claim.Type, ClaimValue = claim.Value });

            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult<IList<Claim>>(((Role)role).RoleClaims.Select(x => x.ToClaim()).ToList());
        }

        public Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            ((Role)role).RoleClaims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

            return Task.CompletedTask;
        }
        
        #endregion

    }
    
}