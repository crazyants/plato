using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _entityStore;
        
        public TopicListViewComponent(IContextFacade contextFacade, IEntityStore<Topic> entityStore)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            FilterOptions filterOpts,
            PagerOptions pagerOpts)
        {
       
            var model = await GetIndexViewModel(filterOpts, pagerOpts);

            return View(model);
        }



        async Task<HomeIndexViewModel> GetIndexViewModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var topics = await GetEntities(filterOptions, pagerOptions);
            return new HomeIndexViewModel(
                topics,
                filterOptions,
                pagerOptions);
        }


        async Task<IPagedResults<Topic>> GetEntities(
            FilterOptions filterOpts,
            PagerOptions pagerOptions)
        {

            // Get current feature (i.e. Plato.Discuss) from area
            var feature = await _contextFacade.GetFeatureByAreaAsync();

            return await _entityStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityQueryParams>(q =>
                {
                    
                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    if (filterOpts.ChannelId > 0)
                    {
                        q.CategoryId.Equals(filterOpts.ChannelId);
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
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }
        

    }


}

