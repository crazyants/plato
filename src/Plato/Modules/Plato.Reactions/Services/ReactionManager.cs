using System;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Reactions.Models;
using Plato.Reactions.Stores;

namespace Plato.Reactions.Services
{
    
    public class ReactionManager : IReactionManager
    {

        private readonly IContextFacade _contextFacade;
        private readonly IReactionStore<Reaction> _reactionStore;
  
        public ReactionManager(
            IReactionStore<Reaction> reactionStore,
            IContextFacade contextFacade)
        {
            _reactionStore = reactionStore;
            _contextFacade = contextFacade;
        }
        
        public async Task<IActivityResult<Reaction>> CreateAsync(Reaction model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            if (model.Id >= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FeatureId));
            }

            if (String.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            // Update created by
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            model.CreatedUserId = user?.Id ?? 0;
        
            // Create result
            var result = new ActivityResult<Reaction>();

            // Attempt to persist
            var reaction = await _reactionStore.CreateAsync(model);
            if (reaction != null)
            {
                return result.Success(reaction);
            }

            return result.Failed($"An unknown error occurred whilst attempting to create a reaction");

        }

        public async Task<IActivityResult<Reaction>> UpdateAsync(Reaction model)
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
            
            if (model.FeatureId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.FeatureId));
            }

            if (String.IsNullOrEmpty(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Update modified 
            var user = await _contextFacade.GetAuthenticatedUserAsync();
            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTimeOffset.UtcNow;

            // Create result
            var result = new ActivityResult<Reaction>();

            // Attempt to persist
            var reaction = await _reactionStore.UpdateAsync(model);
            if (reaction != null)
            {
                return result.Success(reaction);
            }

            return result.Failed($"An unknown error occurred whilst attempting to create a reaction");

        }

    }
}
