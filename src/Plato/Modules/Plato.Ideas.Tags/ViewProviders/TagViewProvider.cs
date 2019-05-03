using System;
using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Ideas.Tags.Models;
using Plato.Ideas.Tags.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Tags.Models;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;

namespace Plato.Ideas.Tags.ViewProviders
{
    public class TagViewProvider : BaseViewProvider<Tag>
    {
        
        private readonly IFeatureFacade _featureFacade;
        private readonly ITagStore<Tag> _tagStore;

        public TagViewProvider(
            IFeatureFacade featureFacade,
            ITagStore<Tag> tagStore)
        {
            _featureFacade = featureFacade;
            _tagStore = tagStore;
        }

        #region "Imlementation"
        
        public override Task<IViewProviderResult> BuildIndexAsync(Tag tag, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(TagIndexViewModel<Tag>)] as TagIndexViewModel<Tag>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(TagIndexViewModel<Tag>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<TagIndexViewModel<Tag>>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<TagIndexViewModel<Tag>>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<TagIndexViewModel<Tag>>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Tag tag, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Idea>)] as EntityIndexViewModel<Idea>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Idea>).ToString()} has not been registered on the HttpContext!");
            }

            var indexViewModel = new TagDisplayViewModel
            {
                Options = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas");
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
                View<TagBase>("Home.Display.Header", model => tag).Zone("header").Order(1),
                View<TagBase>("Home.Display.Tools", model => tag).Zone("tools").Order(1),
                View<TagDisplayViewModel>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<TagsViewModel<Tag>>("Question.Tags.Index.Sidebar", model =>
                {
                    model.SelectedTagId = tag?.Id ?? 0;
                    model.Tags = tags?.Data;
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
