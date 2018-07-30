using System.Threading.Tasks;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;
using Plato.Discuss.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Labels.Stores;

namespace Plato.Discuss.Labels.ViewProviders
{
    public class LabelsViewProvider : BaseViewProvider<Label>
    {

        private readonly ILabelStore<Models.Label> _labelStore;
        private readonly IContextFacade _contextFacade;

        public LabelsViewProvider(
            ILabelStore<Label> labelStore,
            IContextFacade contextFacade)
        {
            _labelStore = labelStore;
            _contextFacade = contextFacade;
        }
        
        #region "Imlementation"


        public override async Task<IViewProviderResult> BuildIndexAsync(Label label, IUpdateModel updater)
        {

            // filter options
            var filterOptions = new FilterOptions
            {
                ChannelId = label?.Id ?? 0
            };

            // paging otptions
            var pagerOptions = new PagerOptions
            {
                Page = GetPageIndex(updater)
            };

            var indexViewModel = new LabelIndexViewModel
            {
                FilterOpts = filterOptions,
                PagerOpts = pagerOptions
            };

            // Ensure we explictly set the featureId
            var feature = await _contextFacade.GetFeatureByModuleIdAsync("Plato.Discuss.Labels");
            if (feature == null)
            {
                return default(IViewProviderResult);
            }

            var labels = await _labelStore.GetByFeatureIdAsync(feature.Id);
            
            return Views(
                View<Label>("Home.Index.Header", model => label).Zone("header").Order(1),
                View<Label>("Home.Index.Tools", model => label).Zone("tools").Order(1),
                View<LabelIndexViewModel>("Home.Index.Content", model => indexViewModel).Zone("content").Order(1),
                View<LabelsViewModel>("Discuss.Labels.Index.Sidebar", model =>
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

        #region "Private Methods"

        int GetPageIndex(IUpdateModel updater)
        {

            var page = 1;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("page", out object value);
            if (found)
            {
                int.TryParse(value.ToString(), out page);
            }

            return page;

        }
        
        #endregion

    }
}
