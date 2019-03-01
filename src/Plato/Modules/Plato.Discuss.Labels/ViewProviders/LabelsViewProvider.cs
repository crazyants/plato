using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Discuss.Models;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Stores;
using Plato.Discuss.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;

namespace Plato.Discuss.Labels.ViewProviders
{
    public class LabelViewProvider : BaseViewProvider<Label>
    {

        private readonly ILabelStore<Label> _labelStore;
        private readonly IContextFacade _contextFacade;
        private readonly IFeatureFacade _featureFacade;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LabelViewProvider(
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
        
        public override Task<IViewProviderResult> BuildIndexAsync(Label label, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(LabelIndexViewModel)] as LabelIndexViewModel;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(LabelIndexViewModel).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<LabelIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<LabelIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<LabelIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override async Task<IViewProviderResult> BuildDisplayAsync(Label label, IViewProviderContext context)
        {

            // Get topic index view model from context
            var viewModel = _actionContextAccessor.ActionContext.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Topic>).ToString()} has not been registered on the HttpContext!");
            }
            
            var indexViewModel = new LabelDisplayViewModel
            {
                Options = viewModel?.Options,
                Pager = viewModel?.Pager
            };

            // Ensure we explicitly set the featureId
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }
            
            // Get labels for feature
            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<LabelQueryParams>(q =>
                {
                    q.FeatureId.Equals(feature.Id);
                })
                .OrderBy("Entities", OrderBy.Desc)
                .ToList();

            return Views(
                View<Label>("Home.Display.Header", model => label).Zone("header").Order(1),
                View<Label>("Home.Display.Tools", model => label).Zone("tools").Order(1),
                View<LabelDisplayViewModel>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<LabelsViewModel>("Topic.Labels.Index.Sidebar", model =>
                {
                    model.SelectedLabelId = label?.Id ?? 0;
                    model.Labels = labels?.Data;
                    return model;
                }).Zone("sidebar").Order(1)
            );


        }

        public override Task<IViewProviderResult> BuildEditAsync(Label model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Label model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        #endregion

    }
}
