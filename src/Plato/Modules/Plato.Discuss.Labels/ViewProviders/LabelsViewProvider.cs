using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Labels.Stores;
using Plato.Discuss.ViewModels;

namespace Plato.Discuss.Labels.ViewProviders
{
    public class LabelsViewProvider : BaseViewProvider<Label>
    {

        private readonly ILabelStore<Models.Label> _labelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LabelsViewProvider(
            ILabelStore<Label> labelStore,
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
        
        public override async Task<IViewProviderResult> BuildIndexAsync(Label label, IUpdateModel updater)
        {

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(TopicIndexViewModel)] as TopicIndexViewModel;
            
            var indexViewModel = new LabelIndexViewModel
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

            var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<Label>("Home.Index.Header", model => label).Zone("header").Order(1),
                View<Label>("Home.Index.Tools", model => label).Zone("tools").Order(1),
                View<LabelIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<LabelsViewModel>("Topic.Labels.Index.Sidebar", model =>
                {
                    model.SelectedLabelId = label?.Id ?? 0;
                    model.Labels = labels;
                    return model;
                }).Zone("sidebar").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Label model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Label model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Label model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        #endregion

    }
}
