using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Shell.Abstractions;

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



        async Task<TopicIndexViewModel> GetIndexViewModel(
            ViewOptions viewOptions,
            PagerOptions pagerOptions)
        {
            var topics = await GetEntities(viewOptions, pagerOptions);
            return new TopicIndexViewModel(
                topics,
                viewOptions,
                pagerOptions);
        }


        async Task<IPagedResults<Topic>> GetEntities(
            ViewOptions viewOpts,
            PagerOptions pagerOptions)
        {

            // Explictly get Plato.Discuss feature, this view component can be 
            // used in different areas (i.e. Plat.Discuss.Channels) s dn't get by area name
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss");

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
                .OrderBy("ModifiedDate", OrderBy.Desc)
                .ToList();
        }
        

    }


}

