using System;
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
using Plato.Internal.Models.Metrics;
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
            var viewsByDate = await _aggregatedEntityMetricsRepository.SelectGroupedByDate("CreatedDate", range.Start, range.End);
        
            // Build model
            var overviewViewModel = new EntitiesOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(range.Start, range.End),
                Replies = replies.MergeIntoRange(range.Start, range.End),
                Views = viewsByDate.MergeIntoRange(range.Start, range.End)
            };

            // Build most viewed view model
            var mostViewedViewModel = await GetMostViewedViewModel(range.Start, range.End);

            // Return view
            return Views(
                View<EntitiesOverviewReportViewModel>("Entities.Overview.Report", model => overviewViewModel).Zone("content").Order(1)
                    .Order(1),
                View<EntityMetricsViewModel<int>>("Entities.TopViews.Report", model => mostViewedViewModel).Zone("content").Order(1)
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


        async Task<EntityMetricsViewModel<int>> GetMostViewedViewModel(DateTimeOffset start, DateTimeOffset end)
        {

            // Get views by id for specified range
            var viewsById = await _aggregatedEntityMetricsRepository.SelectGroupedByInt("EntityId", start, end);

            // Get all entities matching ids
            IPagedResults<Entity> mostViewedEntities = null;
            if (viewsById != null)
            {
                mostViewedEntities = await _entityStore.QueryAsync()
                    .Take(1, 10)
                    .Select<EntityQueryParams>(q =>
                    {
                        q.Id.IsIn(viewsById.Data.Select(d => d.Aggregate).ToArray());
                    })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
            }

            // No results to display
            if (mostViewedEntities?.Data == null)
            {
                return null;
            }

            // Build combined result
            var entityMetrics = new List<AggregatedEntityMetric<int>>();
            foreach (var entity in mostViewedEntities.Data)
            {
                // Get or add aggregate
                var aggregate = viewsById?.Data.FirstOrDefault(m => m.Aggregate == entity.Id) ??
                                new AggregatedCount<int>();
                entityMetrics.Add(new AggregatedEntityMetric<int>(aggregate, entity));
            }

            // Return view model
            return new EntityMetricsViewModel<int>()
            {
                Results = entityMetrics.OrderByDescending(o => o.Aggregate.Count),
            };

        }

    }
    
}
