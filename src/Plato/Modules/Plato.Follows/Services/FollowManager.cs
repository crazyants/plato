using System;
using System.Threading.Tasks;
using Plato.Follows.Models;
using Plato.Follows.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Follows.Services
{

    public class FollowManager : IFollowManager<Follow>
    {

        private readonly IFollowStore<Follow> _followStore;
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public FollowManager(
            IAliasCreator aliasCreator,
            IBroker broker,
            IFollowStore<Follow> followStore)
        {
            _broker = broker;
            _followStore = followStore;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<ICommandResult<Follow>> CreateAsync(Follow model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // If we have an Id we should perform an update not a create
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            // ThingId should be optional
            if (model.ThingId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }

            // We already need a userId
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Invoke FollowCreating subscriptions
            foreach (var handler in _broker.Pub<Follow>(this, "FollowCreating"))
            {
                model = await handler.Invoke(new Message<Follow>(model, this));
            }

            var result = new CommandResult<Follow>();

            var newFollow = await _followStore.CreateAsync(model);
            if (newFollow != null)
            {

                // Invoke FollowCreated subscriptions
                foreach (var handler in _broker.Pub<Follow>(this, "FollowCreated"))
                {
                    newFollow = await handler.Invoke(new Message<Follow>(newFollow, this));
                }

                // Return success
                return result.Success(newFollow);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create a follow."));
            
        }

        public async Task<ICommandResult<Follow>> UpdateAsync(Follow model)
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
            
            // ThingId should be optional
            if (model.ThingId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.ThingId));
            }

            // We already need a userId
            if (model.CreatedUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.CreatedUserId));
            }
            
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Invoke FollowUpdating subscriptions
            foreach (var handler in _broker.Pub<Follow>(this, "FollowUpdating"))
            {
                model = await handler.Invoke(new Message<Follow>(model, this));
            }

            var result = new CommandResult<Follow>();

            var follow = await _followStore.UpdateAsync(model);
            if (follow != null)
            {

                // Invoke FollowUpdated subscriptions
                foreach (var handler in _broker.Pub<Follow>(this, "FollowUpdated"))
                {
                    follow = await handler.Invoke(new Message<Follow>(follow, this));
                }

                // Return success
                return result.Success(follow);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update a follow."));
            
        }

        public async Task<ICommandResult<Follow>> DeleteAsync(Follow model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke FollowDeleting subscriptions
            foreach (var handler in _broker.Pub<Follow>(this, "FollowDeleting"))
            {
                model = await handler.Invoke(new Message<Follow>(model, this));
            }
            
            var result = new CommandResult<Follow>();

            if (await _followStore.DeleteAsync(model))
            {

                // Invoke FollowDeleted subscriptions
                foreach (var handler in _broker.Pub<Follow>(this, "FollowDeleted"))
                {
                    model = await handler.Invoke(new Message<Follow>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete a follow."));
            
        }

        #endregion
        
    }

}
