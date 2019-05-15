using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Repositories.Reputations;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class LoginsByDateChartViewComponent : ViewComponent
    {

        private readonly IAggregatedUserReputationRepository _aggregatedUserReputationRepository;

        public LoginsByDateChartViewComponent(
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

            // Get reputation awarded for visits by grouped by CreatedDate
            var data = await _aggregatedUserReputationRepository.SelectGroupedByNameAsync(
                "Visit",
                "CreatedDate",
                options.Start,
                options.End);

            return View(data.MergeIntoRange(options.Start, options.End));

        }

    }

}
