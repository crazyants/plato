using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Reputations;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Reports.ViewModels;

namespace Plato.Reports.ViewComponents
{
    public class UserRepTopListViewComponent : ViewComponent
    {

        private readonly IAggregatedUserReputationRepository _aggregatedUserReputationRepository;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserRepTopListViewComponent(
            IAggregatedUserReputationRepository aggregatedUserReputationRepository,
            IPlatoUserStore<User> platoUserStore)
        {
            _aggregatedUserReputationRepository = aggregatedUserReputationRepository;
            _platoUserStore = platoUserStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            ReportOptions options,
            ChartOptions chart)
        {
            
            if (options == null)
            {
                options = new ReportOptions();
            }

            if (chart == null)
            {
                chart = new ChartOptions();
            }
            
            return View(new ChartViewModel<IEnumerable<AggregatedModel<int, User>>>()
            {
                Options = chart,
                Data = await SelectUsersByReputationAsync(options)
            });

        }

        async Task<IEnumerable<AggregatedModel<int, User>>> SelectUsersByReputationAsync(ReportOptions options)
        {

            // Get views by id for specified range
            var viewsById = options.FeatureId > 0
                ? await _aggregatedUserReputationRepository.SelectSummedByIntAsync(
                    "CreatedUserId",
                    options.Start,
                    options.End,
                    options.FeatureId)
                : await _aggregatedUserReputationRepository.SelectSummedByIntAsync(
                    "CreatedUserId",
                    options.Start,
                    options.End);

            // Get all entities matching ids
            IPagedResults<User> mostViewedEntities = null;
            if (viewsById != null)
            {
                mostViewedEntities = await _platoUserStore.QueryAsync()
                    .Take(1, 10)
                    .Select<UserQueryParams>(q =>
                    {
                        q.Id.IsIn(viewsById.Data.Select(d => d.Aggregate).ToArray());
                    })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
            }

            // Build combined result
            List<AggregatedModel<int, User>> entityMetrics = null;
            if (mostViewedEntities?.Data != null)
            {
                foreach (var entity in mostViewedEntities.Data)
                {
                    // Get or add aggregate
                    var aggregate = viewsById?.Data.FirstOrDefault(m => m.Aggregate == entity.Id);
                    if (aggregate != null)
                    {
                        if (entityMetrics == null)
                        {
                            entityMetrics = new List<AggregatedModel<int, User>>();
                        }
                        entityMetrics.Add(new AggregatedModel<int, User>(aggregate, entity));
                    }
                }
            }

            return entityMetrics?.OrderByDescending(o => o.Aggregate.Count) ?? null;

        }
        
    }

}
