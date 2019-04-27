using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
using Plato.Reports.Services;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {
        
        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminIndexViewProvider(IDateRangeStorage dateRangeStorage)
        {
            _dateRangeStorage = dateRangeStorage;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);
       
            // Build index view model
            var reportIndexOptions = new ReportIndexOptions()
            {
                Start = range.Start,
                End = range.End
            };
            
            return Task.FromResult(Views(
                View<ReportIndexOptions>("Reports.Admin.Index.Tools", model => reportIndexOptions).Zone("tools")
                    .Order(int.MinValue),
                View<ReportIndexOptions>("Reports.AdminIndex", model => reportIndexOptions).Zone("content")
                    .Order(int.MinValue)
            ));

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

            var model = new ReportIndexOptions();

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
