using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Admin.Models;
using Plato.Entities.Metrics.Models;
using Plato.Entities.Metrics.Repositories;
using Plato.Entities.Metrics.Stores;
using Plato.Entities.Models;
using Plato.Entities.Reporting.ViewModels;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Extensions;
using Plato.Reporting.Services;

namespace Plato.Entities.Reporting.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {

        private readonly IEntityStore<Entity> _entityStore;

        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;
        private readonly IEntityMetricsStore<EntityMetric> _entityMetricStore;

        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminIndexViewProvider(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository,
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository,
            IDateRangeStorage dateRangeStorage, 
            IEntityMetricsStore<EntityMetric> entityMetricStore,
            IEntityStore<Entity> entityStore)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
            _dateRangeStorage = dateRangeStorage;
            _entityMetricStore = entityMetricStore;
            _entityStore = entityStore;
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);

            // Get data
            var entities = await _aggregatedEntityRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
            var replies = await _aggregatedEntityReplyRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
            var views = await _aggregatedEntityMetricsRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);


            var topViews = await _aggregatedEntityMetricsRepository.SelectGroupedByInt("EntityId", range.Start, range.End);
            
            var topEntities = await _entityStore.QueryAsync()
                .Take(1, 10)
                .Select<EntityQueryParams>(q =>
                {
                    q.Id.IsIn(topViews.Data.Select(d => d.Aggregate).ToArray());
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

            var combinedEntityMetrics = new List<AggregatedEntityMetric<int>>();

            foreach (var entity in topEntities.Data)
            {
                var aggregate = topViews?.Data.FirstOrDefault(m => m.Aggregate == entity.Id);
                combinedEntityMetrics.Add(new AggregatedEntityMetric<int>()
                {
                    Aggregate = aggregate,
                    Entity = entity
                });

            }

            var combinedEntityMetricsViewModel = new CombinedEntityMetricsViewModel<int>()
            {
                Results = combinedEntityMetrics.OrderByDescending(o => o.Aggregate.Count),
            };

            // Build model
            var overviewViewModel = new EntitiesOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(range.Start, range.End),
                Replies = replies.MergeIntoRange(range.Start, range.End),
                Views = views.MergeIntoRange(range.Start, range.End)
            };

            // Return view
            return Views(
                View<EntitiesOverviewReportViewModel>("Entities.Overview.Report", model => overviewViewModel).Zone("content").Order(1)
                    .Order(1),
                View<CombinedEntityMetricsViewModel<int>>("Entities.TopViews.Report", model => combinedEntityMetricsViewModel).Zone("content").Order(1)
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
            return await BuildEditAsync(viewModel, context);
        }
        
    }
    
}
