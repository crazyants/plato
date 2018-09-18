using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewComponents
{
    public class SearchListViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Entity> _entityStore;
        private readonly IFeatureFacade _featureFacade;
        
        public SearchListViewComponent(
            IContextFacade contextFacade,
            IEntityStore<Entity> entityStore,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _featureFacade = featureFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ViewOptions viewOpts,
            PagerOptions pagerOpts)
        {

            if (viewOpts == null)
            {
                viewOpts = new ViewOptions();
            }

            if (pagerOpts == null)
            {
                pagerOpts = new PagerOptions();
            }
            
            var model = await GetIndexViewModel(viewOpts, pagerOpts);

            return View(model);

        }
        
        async Task<SearchIndexViewModel> GetIndexViewModel(
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            var results = await GetEntities(viewOptions, pagerOptions);
            return new SearchIndexViewModel(
                results,
                viewOptions,
                pagerOptions);
        }


        async Task<IPagedResults<Entity>> GetEntities(
            ViewOptions viewOpts,
            PagerOptions pagerOptions)
        {

            // Explictly get Plato.Discuss feature, this view component can be 
            // used in different areas (i.e. Plat.Discuss.Channels) s dn't get by area name
            var feature = await _featureFacade.GetModuleByIdAsync("Plato.Discuss");

            return await _entityStore.QueryAsync()
                .Take(pagerOptions.Page, pagerOptions.PageSize)
                .Select<EntityQueryParams>(q =>
                {
                    
                    if (feature != null)
                    {
                        q.FeatureId.Equals(feature.Id);
                    }

                    if (viewOpts.ChannelId > 0)
                    {
                        q.CategoryId.Equals(viewOpts.ChannelId);
                    }

                    if (!string.IsNullOrEmpty(viewOpts.Search))
                    {
                        q.Title.Like(viewOpts.Search).Or();
                        q.Message.Like(viewOpts.Search).Or();
                        q.Html.Like(viewOpts.Search).Or();
                    }
                    
                    q.HideSpam.True();
                    q.HidePrivate.True();
                    q.HideDeleted.True();
               
                    // q.UserName.IsIn("Admin,Mark").Or();
                    // q.Email.IsIn("email440@address.com,email420@address.com");
                    // q.Id.Between(1, 5);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
        }
        

    }


}

