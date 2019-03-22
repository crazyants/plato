using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Plato.Entities.Ratings.Models;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Ratings.Stores
{

    public class SimpleRatingsStore : ISimpleRatingsStore
    {

        // Maximum number of users to show within the tooltip text
        private const int ByMax = 15;

        private readonly IStringLocalizer _localizer;
  
        private readonly IEntityRatingsStore<EntityRating> _entityRatingStore;

        public SimpleRatingsStore(
            IStringLocalizer localizer,
       
            IEntityRatingsStore<EntityRating> entityRatingStore)
        {
            _localizer = localizer;
            _entityRatingStore = entityRatingStore;
        }
        
        public async Task<IEnumerable<SimpleRating>> GetSimpleRatingsAsync(int entityId, int entityReplyId)
        {

            var text1 = _localizer["reacted with the"].Value;
            var text2 = _localizer["emoji"].Value;

            var output = new List<SimpleRating>();
            foreach (var rating in await GetRatingsAsync(entityId, entityReplyId))
            {
                // Build information tooltip
                var i = 0;
                var sb = new StringBuilder();
                foreach (var user in rating.Value.Users)
                {
                    sb.Append(user.DisplayName);
                    if (i < rating.Value.Users.Count - 1)
                    {
                        sb.Append(", ");
                    }
                    if (i >= ByMax)
                    {
                        break;
                    }
                    i++;
                }
                sb.Append(" ")
                    .Append(text1)
                    .Append(" ")
                    .Append(rating.Key.ToLower())
                    .Append(" ")
                    .Append(text2);

                // Build simple reaction
                output.Add(new SimpleRating()
                {
                    Rating = rating.Key,
                    Total = rating.Value.Users.Count.ToPrettyInt(),
                    ToolTip = sb.ToString()
                });
            }

            return output;
        }

        async Task<IDictionary<string, RatingEntryGrouped>> GetRatingsAsync(int entityId, int entityReplyId)
        {

            // Get all ratings for the entity (this includes entity replies for perf)
            var entityRatings = await _entityRatingStore.QueryAsync()
                .Select<EntityRatingsQueryParams>(q => { q.EntityId.Equals(entityId); })
                .OrderBy("Id", OrderBy.Asc)
                .ToList();

            var output = new ConcurrentDictionary<string, RatingEntryGrouped>();
            if (entityRatings != null)
            {


                // Iterate all entity ratings matching supplied entityId and entityReplyId
                foreach (var entityRating in entityRatings?.Data.Where(r => r.EntityReplyId == entityReplyId))
                {

                    // Create a dictionary with all users who have rated grouped by rating
                    output.AddOrUpdate(entityRating.Rating.ToString(),
                        new RatingEntryGrouped(new RatingEntry()
                        {
                            CreatedBy = entityRating.CreatedBy,
                            CreatedDate = entityRating.CreatedDate
                        }),
                        (k, v) =>
                        {
                            v.Users.Add(entityRating.CreatedBy);
                            return v;
                        });


                }

            }

            return output;

        }

    }

}
