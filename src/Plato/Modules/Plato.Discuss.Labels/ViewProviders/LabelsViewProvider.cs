using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Data.Abstractions;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;
using Plato.Entities.ViewModels;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Models;

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
        
        public override Task<IViewProviderResult> BuildIndexAsync(Label label, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(LabelIndexViewModel<Label>)] as LabelIndexViewModel<Label>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(LabelIndexViewModel<Label>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<LabelIndexViewModel<Label>>("Home.Index.Header", model => viewModel).Zone("header").Order(1),
                View<LabelIndexViewModel<Label>>("Home.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<LabelIndexViewModel<Label>>("Home.Index.Content", model => viewModel).Zone("content").Order(1)
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
            
            var indexViewModel = new EntityIndexViewModel<Topic>
            {
                Options = viewModel?.Options,
                Pager = viewModel?.Pager
            };
            
            // Get labels for feature
            var labels = await _labelStore.QueryAsync()
                .Take(1, 10)
                .Select<LabelQueryParams>(q =>
                {
                    if (viewModel.Options.FeatureId != null)
                    {
                        q.FeatureId.Equals(viewModel.Options.FeatureId.Value);
                    }
                })
                .OrderBy("Entities", OrderBy.Desc)
                .ToList();

            return Views(
                View<Label>("Home.Display.Header", model => label).Zone("header").Order(1),
                View<Label>("Home.Display.Tools", model => label).Zone("tools").Order(1),
                View<EntityIndexViewModel<Topic>>("Home.Display.Content", model => indexViewModel).Zone("content").Order(1),
                View<LabelsViewModel<Label>>("Topic.Labels.Index.Sidebar", model =>
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
        
    }

}
