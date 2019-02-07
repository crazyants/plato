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

        private readonly IStarStore<Star> _starStore;
        private readonly IBroker _broker;

        public StarManager(
            IBroker broker,
            IStarStore<Star> starStore)
        {
            _broker = broker;
            _starStore = starStore;
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
            
            // Invoke StarCreating subscriptions
            foreach (var handler in _broker.Pub<Star>(this, "StarCreating"))
            {
                model = await handler.Invoke(new Message<Star>(model, this));
            }

            var result = new CommandResult<Star>();

            var newStar = await _starStore.CreateAsync(model);
            if (newStar != null)
            {

                // Invoke StarCreated subscriptions
                foreach (var handler in _broker.Pub<Star>(this, "StarCreated"))
                {
                    newStar = await handler.Invoke(new Message<Star>(newStar, this));
                }

                // Return success
                return result.Success(newStar);

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
            
            // Invoke StarUpdating subscriptions
            foreach (var handler in _broker.Pub<Star>(this, "StarUpdating"))
            {
                model = await handler.Invoke(new Message<Star>(model, this));
            }

            var result = new CommandResult<Star>();

            var updatedStar = await _starStore.UpdateAsync(model);
            if (updatedStar != null)
            {

                // Invoke StarUpdated subscriptions
                foreach (var handler in _broker.Pub<Star>(this, "StarUpdated"))
                {
                    updatedStar = await handler.Invoke(new Message<Star>(updatedStar, this));
                }

                // Return success
                return result.Success(updatedStar);
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
            
            // Invoke StarDeleting subscriptions
            foreach (var handler in _broker.Pub<Star>(this, "StarDeleting"))
            {
                model = await handler.Invoke(new Message<Star>(model, this));
            }
            
            var result = new CommandResult<Star>();

            if (await _starStore.DeleteAsync(model))
            {

                // Invoke StarDeleted subscriptions
                foreach (var handler in _broker.Pub<Star>(this, "StarDeleted"))
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
