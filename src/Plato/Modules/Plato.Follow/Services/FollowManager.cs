using System;
using System.Threading.Tasks;
using Plato.Follow.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Follow.Services
{

    public class FollowManager : IFollowManager<Models.Follow>
    {

        private readonly IFollowStore<Models.Follow> _followStore;
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public FollowManager(
            IAliasCreator aliasCreator,
            IBroker broker,
            IFollowStore<Models.Follow> followStore)
        {
            _broker = broker;
            _followStore = followStore;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<ICommandResult<Models.Follow>> CreateAsync(Models.Follow model)
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

            if (model.ThingId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Invoke FollowCreating subscriptions
            foreach (var handler in _broker.Pub<Models.Follow>(this, "FollowCreating"))
            {
                model = await handler.Invoke(new Message<Models.Follow>(model, this));
            }

            var result = new CommandResult<Models.Follow>();

            var follow = await _followStore.CreateAsync(model);
            if (follow != null)
            {

                // Invoke FollowCreated subscriptions
                foreach (var handler in _broker.Pub<Models.Follow>(this, "FollowCreated"))
                {
                    follow = await handler.Invoke(new Message<Models.Follow>(follow, this));
                }

                // Return success
                return result.Success(follow);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create a follow."));
            
        }

        public async Task<ICommandResult<Models.Follow>> UpdateAsync(Models.Follow model)
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
            
            if (model.ThingId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }

            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Invoke FollowUpdating subscriptions
            foreach (var handler in _broker.Pub<Models.Follow>(this, "FollowUpdating"))
            {
                model = await handler.Invoke(new Message<Models.Follow>(model, this));
            }

            var result = new CommandResult<Models.Follow>();

            var follow = await _followStore.UpdateAsync(model);
            if (follow != null)
            {

                // Invoke FollowUpdated subscriptions
                foreach (var handler in _broker.Pub<Models.Follow>(this, "FollowUpdated"))
                {
                    follow = await handler.Invoke(new Message<Models.Follow>(follow, this));
                }

                // Return success
                return result.Success(follow);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update a follow."));
            
        }

        public async Task<ICommandResult<Models.Follow>> DeleteAsync(Models.Follow model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke FollowDeleting subscriptions
            foreach (var handler in _broker.Pub<Models.Follow>(this, "FollowDeleting"))
            {
                model = await handler.Invoke(new Message<Models.Follow>(model, this));
            }
            
            var result = new CommandResult<Models.Follow>();

            if (await _followStore.DeleteAsync(model))
            {

                // Invoke FollowDeleted subscriptions
                foreach (var handler in _broker.Pub<Models.Follow>(this, "FollowDeleted"))
                {
                    model = await handler.Invoke(new Message<Models.Follow>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete a follow."));
            
        }

        #endregion
        
    }

}
