using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Reputations.Models;
using Plato.Reputations.Stores;

namespace Plato.Reputations.Services
{

    public interface IUserReputationManager<TUserReputation> : ICommandManager<TUserReputation> where TUserReputation : class
    {

    }

    public class UserReputationManager : IUserReputationManager<UserReputation>
    {

        private readonly IUserReputationsStore<UserReputation> _userReputationStore;
        private readonly IBroker _broker;
        private readonly IContextFacade _contextFacade;

        public UserReputationManager(
            IUserReputationsStore<UserReputation> userReputationStore, 
            IBroker broker, 
            IContextFacade contextFacade)
        {
            _userReputationStore = userReputationStore;
            _broker = broker;
            _contextFacade = contextFacade;
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

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.CreatedUserId == 0)
            {
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

        public async Task<ICommandResult<UserReputation>> DeleteAsync(UserReputation model)
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

        public Task<ICommandResult<UserReputation>> UpdateAsync(UserReputation model)
        {
            throw new NotImplementedException();
        }
    }
}
