using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Labels.Models;
using Plato.Labels.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Text.Abstractions;

namespace Plato.Labels.Services
{

    public class LabelManager<TLabel> : ILabelManager<TLabel> where TLabel : class, ILabel
    {
        
        
        private readonly ILabelRoleStore<LabelRole> _LabelRoleStore;
        private readonly ILabelDataStore<LabelData> _LabelDataStore;
        private readonly ILabelStore<TLabel> _LabelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;
        private readonly IPlatoRoleStore _roleStore;
        private readonly IBroker _broker;

        public LabelManager(
            ILabelStore<TLabel> LabelStore,
            ILabelRoleStore<LabelRole> LabelRoleStore,
            ILabelDataStore<LabelData> LabelDataStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator,
            IPlatoRoleStore roleStore,
            IBroker broker)
        {
            _LabelStore = LabelStore;
            _LabelRoleStore = LabelRoleStore;
            _roleStore = roleStore;
            _contextFacade = contextFacade;
            _LabelDataStore = LabelDataStore;
            _broker = broker;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<IActivityResult<TLabel>> CreateAsync(TLabel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }
            
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Configure model

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.CreatedUserId == 0)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }
            
            model.CreatedDate = DateTime.UtcNow;
            model.Alias = await ParseAlias(model.Name);
          
            // Publish LabelCreating event
            await _broker.Pub<TLabel>(this, new MessageOptions()
            {
                Key = "LabelCreating"
            }, model);

            var result = new ActivityResult<TLabel>();

            var Label = await _LabelStore.CreateAsync(model);
            if (Label != null)
            {
                // Publish LabelCreated event
                await _broker.Pub<TLabel>(this, new MessageOptions()
                {
                    Key = "LabelCreated"
                }, Label);
                // Return success
                return result.Success(Label);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create the Label"));
            
        }

        public async Task<IActivityResult<TLabel>> UpdateAsync(TLabel model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }
            
            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Configure model

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTime.UtcNow;
            model.Alias = await ParseAlias(model.Name);
            
            // Publish LabelUpdating event
            await _broker.Pub<TLabel>(this, new MessageOptions()
            {
                Key = "LabelUpdating"
            }, model);

            var result = new ActivityResult<TLabel>();

            var Label = await _LabelStore.UpdateAsync(model);
            if (Label != null)
            {
                // Publish LabelUpdated event
                await _broker.Pub<TLabel>(this, new MessageOptions()
                {
                    Key = "LabelUpdated"
                }, Label);
                // Return success
                return result.Success(Label);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to update the Label"));
            
        }

        public async Task<IActivityResult<TLabel>> DeleteAsync(TLabel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Publish LabelDeleting event
            await _broker.Pub<TLabel>(this, new MessageOptions()
            {
                Key = "LabelDeleting"
            }, model);

            var result = new ActivityResult<TLabel>();
            if (await _LabelStore.DeleteAsync(model))
            {
                // Delete Label roles
                await _LabelRoleStore.DeleteByLabelIdAsync(model.Id);

                // Delete Label data
                var data = await _LabelDataStore.GetByLabelIdAsync(model.Id);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        await _LabelDataStore.DeleteAsync(item);
                    }
                    
                }

                // Publish LabelDeleted event
                await _broker.Pub<TLabel>(this, new MessageOptions()
                {
                    Key = "LabelDeleted"
                }, model);

                // Return success
                return result.Success();

            }
            
            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to delete the Label"));


        }

        public async Task<IActivityResult<TLabel>> AddToRoleAsync(TLabel model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var result = new ActivityResult<TLabel>();

            // Ensure the role exists
            var existingRole = await _roleStore.GetByNameAsync(roleName);
            if (existingRole == null)
            {
                return result.Failed(new ActivityError($"A role with the name {roleName} could not be found"));
            }

            // Ensure supplied role name is not already associated with the Label
            var roles = await _LabelRoleStore.GetByLabelIdAsync(model.Id);
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return result.Failed(new ActivityError($"A role with the name '{roleName}' is already associated with the Label '{model.Name}'"));
                    }
                }
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
        
            var LabelRole = new LabelRole()
            {
                LabelId = model.Id,
                RoleId = existingRole.Id,
                CreatedUserId = user?.Id ?? 0
            };

            var updatedLabelRole = await _LabelRoleStore.CreateAsync(LabelRole);
            if (updatedLabelRole != null)
            {
                return result.Success();
            }

            return result.Failed(new ActivityError($"An unknown error occurred whilst attempting to add role '{existingRole.Name}' for Label '{model.Name}'"));

        }

        public async Task<IActivityResult<TLabel>> RemoveFromRoleAsync(TLabel model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }
            
            // Our result
            var result = new ActivityResult<TLabel>();
            
            // Ensure the role exists
            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return result.Failed(new ActivityError($"A role with the name {roleName} could not be found"));
            }
            
            // Attempt to delete the role relationship
            var success = await _LabelRoleStore.DeleteByRoleIdAndLabelIdAsync(role.Id, model.Id);
            if (success)
            {
                return result.Success();
            }
            
            return result.Failed(new ActivityError($"An unknown error occurred whilst attempting to remove role '{role.Name}' for Label '{model.Name}'"));

        }

        public async Task<bool> IsInRoleAsync(TLabel model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var result = new ActivityResult<TLabel>();

            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return false;
            }
            
            var roles = await _LabelRoleStore.GetByLabelIdAsync(model.Id);
            if (roles == null)
            {
                return false;
            }

            foreach (var localRole in roles)
            {
                if (localRole.Id == role.Id)
                {
                    return true;
                }
            }

            return false;

        }

        public async Task<IEnumerable<string>> GetRolesAsync(TLabel model)
        {

            var roles = await _LabelRoleStore.GetByLabelIdAsync(model.Id);
            if (roles != null)
            {
                return roles.Select(s => s.RoleName).ToArray();
            }

            return new string[] {};

        }

        #endregion

        #region "Private Methods"

        private async Task<string> ParseAlias(string input)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseLabelAlias"
            }, input))
            {
                return await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return _aliasCreator.Create(input);

        }

        #endregion

    }

}
