using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Moderation.Extensions
{

    public static class ModeratorStoreExtensions
    {

        public static async Task<IDictionary<SimpleUser, IEnumerable<Moderator>>> GetCategorizedModeratorsAsync(this IModeratorStore<Moderator> moderatorStore)
        {

            // Get all moderators
            var moderators = await moderatorStore
                .QueryAsync()
                .ToList();

            if (moderators?.Data == null)
            {
                return null;
            }

            // Build distinct list of moderators
            IDictionary<int, SimpleUser> users = null;
            foreach (var moderator in moderators.Data)
            {
                if (users == null)
                {
                    users = new ConcurrentDictionary<int, SimpleUser>();
                }
                if (!users.Keys.Contains(moderator.UserId))
                {
                    users.Add(moderator.UserId, moderator.User);
                }
            }

            if (users == null)
            {
                return null;
            }

            // Add moderator entries for each distinct user
            var output = new ConcurrentDictionary<SimpleUser, IEnumerable<Moderator>>();
            foreach (var user in users.Values)
            {
                var entries = moderators.Data
                    .Where(m => m.UserId == user.Id)
                    .ToList();
                foreach (var entry in entries)
                {
                    entry.CategoryName = "All Channels";
                    if (output.ContainsKey(user))
                    {
                        output[user] = output[user].Concat(new[] { entry });
                    }
                    else
                    {
                        output.TryAdd(user, new[] { entry });
                    }
                }
            }

            return output;

        }
        
    }
}
