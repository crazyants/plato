using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Reputations.Abstractions;
using Plato.Internal.Stores.Abstractions.Reputations;

namespace Plato.Internal.Reputations
{
    
    public class UserReputationManager : IUserReputationManager<UserReputation>
    {

        private readonly IUserReputationsStore<UserReputation> _userReputationStore;
        private readonly IContextFacade _contextFacade;
        private readonly IBroker _broker;

        public UserReputationManager(
            IUserReputationsStore<UserReputation> userReputationStore,             
            IContextFacade contextFacade,
            IBroker broker)
        {
            _userReputationStore = userReputationStore;            
            _contextFacade = contextFacade;
            _broker = broker;
        }

        public async Task<ICommandResult<UserReputation>> CreateAsync(UserReputation model)
        {
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            // Configure model
            if (model.CreatedUserId == 0)
            {
                var user = await _contextFacade.GetAuthenticatedUserAsync();
                model.CreatedUserId = user?.Id ?? 0;
            }
            
            // Invoke UserReputationCreating subscriptions
            foreach (var handler in _broker.Pub<UserReputation>(this, "UserReputationCreating"))
            {
                model = await handler.Invoke(new Message<UserReputation>(model, this));
            }

            var result = new CommandResult<UserReputation>();

            var newUserReputation = await _userReputationStore.CreateAsync(model);
            if (newUserReputation != null)
            {

                // Invoke UserReputationCreated subscriptions
                foreach (var handler in _broker.Pub<UserReputation>(this, "UserReputationCreated"))
                {
                    newUserReputation = await handler.Invoke(new Message<UserReputation>(newUserReputation, this));
                }

                // Return success
                return result.Success(newUserReputation);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the user reputation entry."));

        }
        
        public async Task<ICommandResult<UserReputation>> UpdateAsync(UserReputation model)
        {
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // Invoke UserReputationUpdating subscriptions
            foreach (var handler in _broker.Pub<UserReputation>(this, "UserReputationUpdating"))
            {
                model = await handler.Invoke(new Message<UserReputation>(model, this));
            }

            var result = new CommandResult<UserReputation>();

            var updatedUserReputation = await _userReputationStore.UpdateAsync(model);
            if (updatedUserReputation != null)
            {

                // Invoke UserReputationUpdated subscriptions
                foreach (var handler in _broker.Pub<UserReputation>(this, "UserReputationUpdated"))
                {
                    updatedUserReputation = await handler.Invoke(new Message<UserReputation>(updatedUserReputation, this));
                }

                // Return success
                return result.Success(updatedUserReputation);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create the user reputation entry."));

        }


        public async Task<ICommandResult<UserReputation>> DeleteAsync(UserReputation model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Invoke UserReputationDeleting subscriptions
            foreach (var handler in _broker.Pub<UserReputation>(this, "UserReputationDeleting"))
            {
                model = await handler.Invoke(new Message<UserReputation>(model, this));
            }

            var result = new CommandResult<UserReputation>();
            var success = await _userReputationStore.DeleteAsync(model);
            if (success)
            {

                // Invoke UserReputationDeleted subscriptions
                foreach (var handler in _broker.Pub<UserReputation>(this, "UserReputationDeleted"))
                {
                    model = await handler.Invoke(new Message<UserReputation>(model, this));
                }

                // Return success
                return result.Success();

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete the user reputation entry"));
            
        }

    }

}
