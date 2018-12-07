using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Discuss.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewProviders
{
    public class TagViewProvider : BaseViewProvider<Tag>
    {

        private readonly ITagStore<Tag> _labelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;

        public TagViewProvider(
            ITagStore<Tag> labelStore,
            IContextFacade contextFacade,
            IFeatureFacade featureFacade,
            IActionContextAccessor actionContextAccessor)
        {
            _labelStore = labelStore;
            _contextFacade = contextFacade;
            _featureFacade = featureFacade;
            _actionContextAccessor = actionContextAccessor;
        }
        
        #region "Imlementation"
        
        public override Task<IViewProviderResult> BuildIndexAsync(Tag label, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(TagIndexViewModel)] as TagIndexViewModel;
         
            return Task.FromResult(Views(
                View<TagIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<TagIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<TagIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Tag label, IViewProviderContext context)
        {

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(TopicIndexViewModel)] as TopicIndexViewModel;

            var indexViewModel = new TagDisplayViewModel
            {
                TopicIndexOpts = viewModel?.Options,
                PagerOpts = viewModel?.Pager
            };

            // Ensure we explictly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            //var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);

            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<TagQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("Entities", OrderBy.Desc)
                .ToList();

            return Views(
                View<Tag>("Home.Display.Header", model => label).Zone("header").Order(1),
                View<Tag>("Home.Display.Tools", model => label).Zone("tools").Order(1),
                View<TagDisplayViewModel>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<TagsViewModel>("Topic.Labels.Index.Sidebar", model =>
                {
                    model.SelectedLabelId = label?.Id ?? 0;
                    model.Tags = labels?.Data;
                    return model;
                }).Zone("sidebar").Order(1)
            );


        }

        public override Task<IViewProviderResult> BuildEditAsync(Tag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Tag model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        #endregion

    }
}
