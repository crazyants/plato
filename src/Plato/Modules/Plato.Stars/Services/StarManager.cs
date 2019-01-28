using System;
using System.Threading.Tasks;
using Plato.Stars.Models;
using Plato.Stars.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Text.Abstractions;

namespace Plato.Stars.Services
{

    public class StarManager : IStarManager<Star>
    {

        private readonly IStarStore<Star> _followStore;
        private readonly IAliasCreator _aliasCreator;
        private readonly IBroker _broker;

        public StarManager(
            IAliasCreator aliasCreator,
            IBroker broker,
            IStarStore<Star> followStore)
        {
            _broker = broker;
            _followStore = followStore;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<ICommandResult<Star>> CreateAsync(Star model)
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
            foreach (var handler in _broker.Pub<Star>(this, "FollowCreating"))
            {
                model = await handler.Invoke(new Message<Star>(model, this));
            }

            var result = new CommandResult<Star>();

            var newFollow = await _followStore.CreateAsync(model);
            if (newFollow != null)
            {

                // Invoke FollowCreated subscriptions
                foreach (var handler in _broker.Pub<Star>(this, "FollowCreated"))
                {
                    newFollow = await handler.Invoke(new Message<Star>(newFollow, this));
                }

                // Return success
                return result.Success(newFollow);

            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to create a follow."));
            
        }

        public async Task<ICommandResult<Star>> UpdateAsync(Star model)
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
            foreach (var handler in _broker.Pub<Star>(this, "FollowUpdating"))
            {
                model = await handler.Invoke(new Message<Star>(model, this));
            }

            var result = new CommandResult<Star>();

            var follow = await _followStore.UpdateAsync(model);
            if (follow != null)
            {

                // Invoke FollowUpdated subscriptions
                foreach (var handler in _broker.Pub<Star>(this, "FollowUpdated"))
                {
                    follow = await handler.Invoke(new Message<Star>(follow, this));
                }

                // Return success
                return result.Success(follow);
            }

            return result.Failed(new CommandError("An unknown error occurred whilst attempting to update a follow."));
            
        }

        public async Task<ICommandResult<Star>> DeleteAsync(Star model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            // Invoke FollowDeleting subscriptions
            foreach (var handler in _broker.Pub<Star>(this, "FollowDeleting"))
            {
                model = await handler.Invoke(new Message<Star>(model, this));
            }
            
            var result = new CommandResult<Star>();

            if (await _followStore.DeleteAsync(model))
            {

                // Invoke FollowDeleted subscriptions
                foreach (var handler in _broker.Pub<Star>(this, "FollowDeleted"))
                {
                    model = await handler.Invoke(new Message<Star>(model, this));
                }

                // Return success
                return result.Success();

            }
            
            return result.Failed(new CommandError("An unknown error occurred whilst attempting to delete a follow."));
            
        }

        #endregion
        
    }

}
