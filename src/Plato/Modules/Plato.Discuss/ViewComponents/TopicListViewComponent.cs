using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Roles;

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
            int pageIndex,
            int pageSize)
        {
            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions();
            pagerOptions.Page = pageIndex;

            var model = await GetIndexViewModel(filterOptions, pagerOptions);

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
            FilterOptions filterOptions,
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

