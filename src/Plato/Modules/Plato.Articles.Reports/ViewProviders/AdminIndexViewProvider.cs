using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Reports.Services;
using Plato.Reports.ViewModels;

namespace Plato.Articles.Reports.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {
        
        private readonly IDateRangeStorage _dateRangeStorage;
        private readonly IFeatureFacade _featureFacade;

        public AdminIndexViewProvider(
            IDateRangeStorage dateRangeStorage,
            IFeatureFacade featureFacade)
        {
            _dateRangeStorage = dateRangeStorage;
            _featureFacade = featureFacade;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Articles");

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
            
            // Build index view model
            var reportIndexViewModel = new ReportOptions()
            {
                Start = range.Start,
                End = range.End,
                FeatureId = feature?.Id ?? 0
            };

            return Views(
                View<ReportOptions>("Reports.Admin.Index.Tools", model => reportIndexViewModel).Zone("tools")
                    .Order(int.MinValue),
                View<ReportOptions>("Reports.Articles.AdminIndex", model => reportIndexViewModel).Zone("content").Order(1)
                    .Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            var model = new ReportOptions();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(viewModel, context);
            }

            if (context.Updater.ModelState.IsValid)
            {
                var storage = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
                storage.Set(model.Start, model.End);
            }

            return await BuildIndexAsync(viewModel, context);

        }
        
    }
    
}
