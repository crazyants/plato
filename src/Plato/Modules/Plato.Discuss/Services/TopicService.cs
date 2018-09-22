using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Services
{
    
    public interface ITopicService
    {
        Task<IPagedResults<Topic>> Get(TopicIndexOptions options, PagerOptions pager);

    }

    public class TopicService : ITopicService
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicService(
            IContextFacade contextFacade,
            IEntityStore<Topic> topicStore,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _topicStore = topicStore;
            _featureFacade = featureFacade;
        }

        public async Task<IPagedResults<Topic>> Get(TopicIndexOptions options, PagerOptions pager)
        {

            if (options == null)
            {
                options = new TopicIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }

            // Get discuss feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");

            // Get authenticated user
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            // Return tailored results
            return await _topicStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<EntityQueryParams>(q =>
                {

                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    //if (user != null)
                    //{
                    //    q.UserId.Equals(user.Id);
                    //}
                    
                    if (options.ChannelId > 0)
                    {
                        q.CategoryId.Equals(options.ChannelId);
                    }

                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();

                    //q.IsPinned.True();
                    
                    //if (!string.IsNullOrEmpty(filterOptions.Search))
                    //{
                    //    q.UserName.IsIn(filterOptions.Search).Or();
                    //    q.Email.IsIn(filterOptions.Search);
                    //}
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy(options.Sort.ToString(), options.Order)
                .ToList();

        }

    }

}
