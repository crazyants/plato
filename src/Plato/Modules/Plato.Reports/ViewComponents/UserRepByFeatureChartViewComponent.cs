using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Reports.ViewModels;
using Plato.Internal.Repositories.Reputations;

namespace Plato.Reports.ViewComponents
{
    public class UserRepByFeatureChartViewComponent : ViewComponent
    {

        private readonly IAggregatedUserReputationRepository _aggregatedUserReputationRepository;

        public UserRepByFeatureChartViewComponent(
            IAggregatedUserReputationRepository aggregatedUserReputationRepository)
        {
            _aggregatedUserReputationRepository = aggregatedUserReputationRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportOptions options)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }
            
            var data = await _aggregatedUserReputationRepository.SelectGroupedByFeature(options.Start, options.End);
            return View(data);

        }

    }

}
