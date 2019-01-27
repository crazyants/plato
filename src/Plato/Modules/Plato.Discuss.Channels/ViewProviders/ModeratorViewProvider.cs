using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Moderation.Models;
using Plato.Moderation.Stores;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class ModeratorViewProvider : BaseViewProvider<Moderator>
    {

        private const string ChannelHtmlName = "channel";

        private readonly ICategoryStore<Channel> _channelStore;
        private readonly IModeratorStore<Moderator> _moderatorStore;
        private readonly HttpRequest _request;
    
        public ModeratorViewProvider(
            IHttpContextAccessor httpContextAccessor, 
            IModeratorStore<Moderator> moderatorStore,
            ICategoryStore<Channel> channelStore)
        {
            _moderatorStore = moderatorStore;
            _channelStore = channelStore;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(Moderator moderator, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Moderator moderator, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Moderator moderator, IViewProviderContext updater)
        {

            var viewModel = new EditTopicChannelsViewModel()
            {
                HtmlName = ChannelHtmlName,
                SelectedChannels = new List<int>
                {
                    moderator.CategoryId
                }
            };

            return Task.FromResult(Views(
                View<EditTopicChannelsViewModel>("Moderation.Channels.Edit.Content", model => viewModel).Zone("sidebar").Order(1)
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Moderator moderator, IUpdateModel updater)
        {
            var valid = await updater.TryUpdateModelAsync(new EditTopicChannelsViewModel()
            {
                SelectedChannels = GetChannelsToAdd()
            });

            return valid;
        }

        public override async Task ComposeTypeAsync(Moderator moderator, IUpdateModel updater)
        {
            
            var model = new EditTopicChannelsViewModel
            {
                SelectedChannels = GetChannelsToAdd()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                moderator.CategoryId = model.SelectedChannels.FirstOrDefault();
            }

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Moderator model, IViewProviderContext context)
        {

            var moderator = await _moderatorStore.GetByIdAsync(model.Id);
            if (moderator == null)
            {
                return await BuildIndexAsync(model, context);
            }
            
            // Validate model
            if (await ValidateModelAsync(model, context.Updater))
            {

                // Get all moderators
                var moderators = await _moderatorStore
                    .QueryAsync()
                    .ToList();

                // Get selected channels
                var selectedCategories = GetChannelsToAdd();

                var output = new List<Moderator>();
                foreach (var categoryId in selectedCategories)
                {

                    // Does the moderator already exists for the category?
                    var existingModerator =
                        moderators.Data.FirstOrDefault(m =>
                            m.UserId == moderator.UserId && m.CategoryId == categoryId);

                    Moderator newOrUpdatedModerator = null;
                    moderator.CategoryId = categoryId;
                    moderator.Claims = GetPostedClaims();

                    // If so update existing moderator
                    if (existingModerator != null)
                    {
                        moderator.Id = existingModerator.Id;
                        newOrUpdatedModerator = await _moderatorStore.UpdateAsync(moderator);
                    }
                    else
                    {
                        moderator.Id = 0;
                        newOrUpdatedModerator = await _moderatorStore.CreateAsync(moderator);
                    }

                    if (newOrUpdatedModerator != null)
                    {
                        output.Add(newOrUpdatedModerator);
                    }
                    
                }

                // If we have channels, lets delete the "All Channels" entry
                if (output.Count > 0)
                {
                    var allChannels = moderators?.Data.FirstOrDefault(m =>
                        m.UserId == moderator.UserId && m.CategoryId == 0);
                    if (allChannels != null)
                    {
                        await _moderatorStore.DeleteAsync(allChannels);
                    }
                }
                

            }

            return await BuildEditAsync(moderator, context);

        }

        #endregion

        #region "Private Methods"


        IList<ModeratorClaim> GetPostedClaims()
        {
            // Build a list of claims to add or update
            var moderatorClaims = new List<ModeratorClaim>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith("Checkbox.") && _request.Form[key] == "true")
                {
                    var permissionName = key.Substring("Checkbox.".Length);
                    moderatorClaims.Add(new ModeratorClaim { ClaimType = ModeratorPermission.ClaimTypeName, ClaimValue = permissionName });
                }
            }

            return moderatorClaims;

        }

        List<int> GetChannelsToAdd()
        {
            // Build selected channels
            List<int> channelsToAdd = null;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(ChannelHtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        int.TryParse(value, out var id);
                        if (id > 0)
                        {
                            if (channelsToAdd == null)
                            {
                                channelsToAdd = new List<int>();
                            }
                            if (!channelsToAdd.Contains(id))
                            {
                                channelsToAdd.Add(id);
                            }
                        }
                       
                    }
                }
            }

            return channelsToAdd;
        }
        
        async Task<IDictionary<SimpleUser, IEnumerable<string>>> GetCategorizedModeratorsAsync()
        {

            // Get all moderator entires
            var moderators = await _moderatorStore.QueryAsync()
                .OrderBy("CreatedDate", OrderBy.Asc)
                .ToList();
            if (moderators == null)
            {
                return null;
            }


            //// Build distrinct list of moderators
            //IDictionary<int, SimpleUser> users = null;
            //foreach (var moderator in moderators.Data)
            //{
            //    if (users == null)
            //    {
            //        users = new ConcurrentDictionary<int, SimpleUser>();
            //    }

            //    if (!users.Keys.Contains(moderator.UserId))
            //    {
            //        users.Add(moderator.UserId, moderator.User);
            //    }
            //}
            

            var output = new ConcurrentDictionary<SimpleUser, IEnumerable<string>>();
            foreach (var moderator in moderators.Data)
            {

                // Attemptt to get channel for moderator
                Channel channel = null;
                if (moderator.CategoryId > 0)
                {
                    channel = await _channelStore.GetByIdAsync(moderator.CategoryId);
                }

                var channelName = "";
                if (channel != null)
                {
                    channelName = channel.Name;
                }

                if (output.ContainsKey(moderator.User))
                {
                    output[moderator.User] = output[moderator.User].Concat(new[] { channelName });
                }
                else
                {
                    output.TryAdd(moderator.User, new[] { channelName });
                }
            }




            return output;
        }


        #endregion

    }

}
