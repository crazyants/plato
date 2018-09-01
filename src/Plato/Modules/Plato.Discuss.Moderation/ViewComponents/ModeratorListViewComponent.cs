using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Moderation.ViewComponents
{
    public class ModeratorListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IModeratorStore<Moderator> _moderatorStore;

        public ModeratorListViewComponent(
            IContextFacade contextFacade, 
            IModeratorStore<Moderator> moderatorStore)
        {
            _contextFacade = contextFacade;
            _moderatorStore = moderatorStore;
        }

        #region "Implementation"

        public async Task<IViewComponentResult> InvokeAsync(
            FilterOptions filterOpts,
            PagerOptions pagerOpts)
        {

            if (filterOpts == null)
            {
                filterOpts = new FilterOptions();
            }
            
            if (pagerOpts == null)
            {
                pagerOpts = new PagerOptions();
            }

            var moderators = await GetCategorizedModeratorsAsync();
            var model = new ModeratorIndexViewModel()
            {
                CategorizedModerators = moderators
            };
            return View(model);

        }

        #endregion

        #region "Private Methods"
        
        async Task<IDictionary<SimpleUser, IEnumerable<Moderator>>> GetCategorizedModeratorsAsync()
        {

            // Get all moderator entires
            var moderators = await _moderatorStore
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

            // Conver to list to prevent multiple enumerations
            var userList = users.Values.ToList();

            // Order our distinct users
            var sortedUsers= userList.OrderBy(u => u.DisplayName);

            // Add moderator entries for each user
            var output = new ConcurrentDictionary<SimpleUser, IEnumerable<Moderator>>();
            foreach (var user in sortedUsers)
            {
                var entires = moderators.Data
                    .Where(m => m.UserId == user.Id)
                    .ToList();
                foreach (var entry in entires)
                {
                    entry.CategoryName = "All Channels";
                    if (output.ContainsKey(user))
                    {
                        output[user] = output[user].Concat(new [] { entry });
                    }
                    else
                    {
                        output.TryAdd(user, new[] {entry});
                    }
                }
            }
            
            return output;

        }
        
        #endregion

    }
    
}

