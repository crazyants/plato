using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Models;
using Plato.Discuss.Tags.Models;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Discuss.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewProviders
{
    public class TagViewProvider : BaseViewProvider<DiscussTag>
    {

        private readonly ITagStore<Tag> _tagStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;

        public TagViewProvider(
            ITagStore<Tag> tagStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IActionContextAccessor actionContextAccessor)
        {
            _tagStore = tagStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _actionContextAccessor = actionContextAccessor;
        }
        
        #region "Imlementation"
        
        public override Task<IViewProviderResult> BuildIndexAsync(DiscussTag tag, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(TagIndexViewModel)] as TagIndexViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(TagIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<TagIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<TagIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<TagIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(DiscussTag tag, IViewProviderContext context)
        {

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Topic>).ToString()} has not been registered on the HttpContext!");
            }

            var indexViewModel = new TagDisplayViewModel
            {
                Options = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Get all tags for feature
            var tags = await _tagStore.QueryAsync()
                .Take(1, 20)
                .Select<TagQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("TotalEntities", OrderBy.Desc)
                .ToList();

            // Build view
            return Views(
                View<Tag>("Home.Display.Header", model => tag).Zone("header").Order(1),
                View<Tag>("Home.Display.Tools", model => tag).Zone("tools").Order(1),
                View<TagDisplayViewModel>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<TagsViewModel>("Topic.Tags.Index.Sidebar", model =>
                {
                    model.SelectedTagId = tag?.Id ?? 0;
                    model.Tags = tags?.Data;
                    return model;
                }).Zone("sidebar").Order(1)
            );
            
        }

        public override Task<IViewProviderResult> BuildEditAsync(DiscussTag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(DiscussTag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        #endregion

    }

}
