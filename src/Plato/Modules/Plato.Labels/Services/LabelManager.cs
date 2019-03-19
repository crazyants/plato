using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Text.Abstractions;
using Plato.Labels.Models;
using Plato.Labels.Stores;

namespace Plato.Labels.Services
{

    public class LabelManager<TLabel> : ILabelManager<TLabel> where TLabel : class, ILabel
    {
        
        private readonly ILabelRoleStore<LabelRole> _labelRoleStore;
        private readonly ILabelDataStore<LabelData> _labelDataStore;
        private readonly ILabelStore<TLabel> _labelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;
        private readonly IPlatoRoleStore _roleStore;
        private readonly IBroker _broker;

        public LabelManager(
            ILabelStore<TLabel> labelStore,
            ILabelRoleStore<LabelRole> labelRoleStore,
            ILabelDataStore<LabelData> labelDataStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator,
            IPlatoRoleStore roleStore,
            IBroker broker)
        {
            _labelStore = labelStore;
            _labelRoleStore = labelRoleStore;
            _roleStore = roleStore;
            _contextFacade = contextFacade;
            _labelDataStore = labelDataStore;
            _broker = broker;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<ICommandResult<TLabel>> CreateAsync(TLabel model)
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

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            model.Alias = await ParseAlias(model.Name);
          
            // Invoke LabelCreating subscriptions
            foreach (var handler in _broker.Pub<TLabel>(this,"LabelCreating"))
            {
                model = await handler.Invoke(new Message<TLabel>(model, this));
            }

            var result = new CommandResult<TLabel>();

            var label = await _labelStore.CreateAsync(model);
            if (label != null)
            {
           
                // Invoke LabelCreated subscriptions
                foreach (var handler in _broker.Pub<TLabel>(this, "LabelCreated"))
                {
                    label = await handler.Invoke(new Message<TLabel>(label, this));
                }

                // Return success
                return result.Success(label);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the Label"));
            
        }

        public async Task<ICommandResult<TLabel>> UpdateAsync(TLabel model)
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
            
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }

            if (model.CreatedDate == null)
            {
                throw new ArgumentNullException(nameof(model.CreatedDate));
            }
            
            // Configure model

            model.Alias = await ParseAlias(model.Name);
            
            // Invoke LabelUpdating subscriptions
            foreach (var handler in _broker.Pub<TLabel>(this, "LabelUpdating"))
            {
                model = await handler.Invoke(new Message<TLabel>(model, this));
            }

            var result = new CommandResult<TLabel>();

            var label = await _labelStore.UpdateAsync(model);
            if (label != null)
            {

                // Invoke LabelUpdated subscriptions
                foreach (var handler in _broker.Pub<TLabel>(this, "LabelUpdated"))
                {
                    label = await handler.Invoke(new Message<TLabel>(label, this));
                }

                // Return success
                return result.Success(label);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update the Label"));
            
        }

        public async Task<ICommandResult<TLabel>> DeleteAsync(TLabel model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke LabelDeleting subscriptions
            foreach (var handler in _broker.Pub<TLabel>(this, "LabelDeleting"))
            {
                model = await handler.Invoke(new Message<TLabel>(model, this));
            }
            
            var result = new CommandResult<TLabel>();
            if (await _labelStore.DeleteAsync(model))
            {
                // Delete Label roles
                await _labelRoleStore.DeleteByLabelIdAsync(model.Id);

                // Delete Label data
                var data = await _labelDataStore.GetByLabelIdAsync(model.Id);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        await _labelDataStore.DeleteAsync(item);
                    }
                }
                
                // Invoke LabelDeleted subscriptions
                foreach (var handler in _broker.Pub<TLabel>(this, "LabelDeleted"))
                {
                    model = await handler.Invoke(new Message<TLabel>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the Label"));


        }

        public async Task<ICommandResult<TLabel>> AddToRoleAsync(TLabel model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var result = new CommandResult<TLabel>();

            // Ensure the role exists
            var existingRole = await _roleStore.GetByNameAsync(roleName);
            if (existingRole == null)
            {
                return result.Failed(new CommandError($"A role with the name {roleName} could not be found"));
            }

            // Ensure supplied role name is not already associated with the Label
            var roles = await _labelRoleStore.GetByLabelIdAsync(model.Id);
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return result.Failed(new CommandError($"A role with the name '{roleName}' is already associated with the Label '{model.Name}'"));
                    }
                }
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
        
            var labelRole = new LabelRole()
            {
                LabelId = model.Id,
                RoleId = existingRole.Id,
                CreatedUserId = user?.Id ?? 0
            };

            var updatedLabelRole = await _labelRoleStore.CreateAsync(labelRole);
            if (updatedLabelRole != null)
            {
                return result.Success();
            }

            return result.Failed(new CommandError($"An unknown error occurred whilst attempting to add role '{existingRole.Name}' for Label '{model.Name}'"));

        }

        public async Task<ICommandResult<TLabel>> RemoveFromRoleAsync(TLabel model, string roleName)
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
            var result = new CommandResult<TLabel>();
            
            // Ensure the role exists
            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return result.Failed(new CommandError($"A role with the name {roleName} could not be found"));
            }
            
            // Attempt to delete the role relationship
            var success = await _labelRoleStore.DeleteByRoleIdAndLabelIdAsync(role.Id, model.Id);
            if (success)
            {
                return result.Success();
            }
            
            return result.Failed(new CommandError($"An unknown error occurred whilst attempting to remove role '{role.Name}' for Label '{model.Name}'"));

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

            var result = new CommandResult<TLabel>();

            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return false;
            }
            
            var roles = await _labelRoleStore.GetByLabelIdAsync(model.Id);
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

            var roles = await _labelRoleStore.GetByLabelIdAsync(model.Id);
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

            var handled = false;
            foreach (var handler in _broker.Pub<string>(this, "ParseLabelAlias"))
            {
                handled = true;
                input =  await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return handled ? input :  _aliasCreator.Create(input);

        }

        #endregion

    }

}
