using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IFeatureFacade _featureFacade;

        public TopicListViewComponent(
            IContextFacade contextFacade, 
            IEntityStore<Topic> entityStore, 
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            TopicIndexOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new TopicIndexOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }


            var model = await GetIndexViewModel(options, pager);

            return View(model);
        }
        
        async Task<TopicIndexViewModel> GetIndexViewModel(
            TopicIndexOptions options,
            PagerOptions pager)
        {
            var topics = await GetEntities(options, pager);
            return new TopicIndexViewModel(
                topics,
                options,
                pager);
        }
        
        async Task<IPagedResults<Topic>> GetEntities(
            TopicIndexOptions topicIndexOpts,
            PagerOptions pagerOptions)
        {
            
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            return await _entityStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityQueryParams>(q =>
                {
                    
                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    if (topicIndexOpts.ChannelId > 0)
                    {
                        q.CategoryId.Equals(topicIndexOpts.ChannelId);
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
                .OrderBy(topicIndexOpts.Sort.ToString(), topicIndexOpts.Order)
                .ToList();
        }
        

    }


}

