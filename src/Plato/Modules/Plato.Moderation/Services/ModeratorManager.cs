using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plato.Internal.Abstractions;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Moderation.Services
{

    public interface IModeratorManager
    {
        Task<ICommandResult<IEnumerable<Moderator>>> CreateAsync(Moderator moderator, IEnumerable<int> categoryIds);


    }

    public class ModeratorManager : IModeratorManager
    {

        private readonly IModeratorStore<Moderator> _moderatorStore;

        public ModeratorManager(IModeratorStore<Moderator> moderatorStore)
        {
            _moderatorStore = moderatorStore;
        }

        public async Task<ICommandResult<IEnumerable<Moderator>>> CreateAsync(Moderator moderator, IEnumerable<int> categoryIds)
        {

            var result = new CommandResult<IEnumerable<Moderator>>();

            // Get all moderators
            var moderators = await _moderatorStore
                .QueryAsync()
                .ToList();
            
            var output = new List<Moderator>();
            foreach (var categoryId in categoryIds)
            {

                // Does the moderator already exists for the category?
                var existingModerator =
                    moderators.Data.FirstOrDefault(m => m.UserId == moderator.UserId && m.CategoryId == categoryId);

                Moderator newOrUpdatedModerator = null;

                // IF so update existing moderator
                if (existingModerator != null)
                {
                    moderator.Id = existingModerator.Id;
                    newOrUpdatedModerator = await _moderatorStore.UpdateAsync(moderator);
                }
                else
                {
                    moderator.CategoryId = categoryId;
                    newOrUpdatedModerator = await _moderatorStore.CreateAsync(moderator);
                }

                if (newOrUpdatedModerator != null)
                {
                    output.Add(newOrUpdatedModerator);
                }
                
            }
         

            return result.Success(output);

        }

    }
}
