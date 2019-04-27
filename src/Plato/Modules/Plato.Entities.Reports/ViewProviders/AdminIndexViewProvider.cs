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
using Plato.Entities.Reports.ViewModels;
using Plato.Entities.Repositories;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Extensions;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Models.Users;
using Plato.Internal.Repositories.Reputations;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Internal.Stores.Users;
using Plato.Reports.Services;
using Plato.Reports.ViewModels;

namespace Plato.Entities.Reports.ViewProviders
{

    public class AdminIndexViewProvider : BaseViewProvider<AdminIndex>
    {

        private readonly IEntityStore<Entity> _entityStore;

        private readonly IAggregatedUserReputationRepository _aggregatedUserReputationRepository;
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;
        private readonly IEntityMetricsStore<EntityMetric> _entityMetricStore;
        private readonly IPlatoUserStore<User> _platoUserStore;

        

        private readonly IDateRangeStorage _dateRangeStorage;

        public AdminIndexViewProvider(
            IAggregatedEntityRepository aggregatedEntityRepository,
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository,
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository,
            IAggregatedUserReputationRepository aggregatedUserReputationRepository,
            IDateRangeStorage dateRangeStorage, 
            IEntityMetricsStore<EntityMetric> entityMetricStore,
            IEntityStore<Entity> entityStore,
            IPlatoUserStore<User> platoUserStore)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
            _aggregatedUserReputationRepository = aggregatedUserReputationRepository;

            _dateRangeStorage = dateRangeStorage;
            _entityMetricStore = entityMetricStore;
            _entityStore = entityStore;
            _platoUserStore = platoUserStore;
            
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel,
            IViewProviderContext context)
        {

            // Get range to display
            var range = _dateRangeStorage.Contextualize(context.Controller.ControllerContext);

            var reportIndexOptions = new ReportIndexOptions()
            {
                Start = range.Start,
                End = range.End
            };

            // Get data
            var entities = await _aggregatedEntityRepository.SelectGroupedByDateAsync("CreatedDate", range.Start, range.End);
            var replies = await _aggregatedEntityReplyRepository.SelectGroupedByDateAsync("CreatedDate", range.Start, range.End);
            var viewsByDate = await _aggregatedEntityMetricsRepository.SelectGroupedByDateAsync("CreatedDate", range.Start, range.End);
        
            // Build model
            var overviewViewModel = new EntitiesOverviewReportViewModel()
            {
                Entities = entities.MergeIntoRange(range.Start, range.End),
                Replies = replies.MergeIntoRange(range.Start, range.End),
                Views = viewsByDate.MergeIntoRange(range.Start, range.End)
            };

            // Build most viewed view model
            var mostViewedViewModel = new TopViewModel()
            {
                Entities = await SelectEntitiesGroupedByViewsAsync(range.Start, range.End),
                Users = await SelectUsersByReputationAsync(range.Start, range.End)
            };

            // Return view
            return Views(
                View<ReportIndexOptions>("Reports.Entities.AdminIndex", model => reportIndexOptions).Zone("content").Order(1)
                    .Order(1),
                View<TopViewModel>("Entities.TopViews.Report", model => mostViewedViewModel).Zone("content").Order(1)
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


        async Task<IEnumerable<AggregatedModel<int, Entity>>> SelectEntitiesGroupedByViewsAsync(DateTimeOffset start, DateTimeOffset end)
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
            List<AggregatedModel<int, Entity>> entityMetrics = null;
            if (mostViewedEntities?.Data != null)
            {
                // Build combined result
                foreach (var entity in mostViewedEntities.Data)
                {
                    // Get or add aggregate
                    var aggregate = viewsById?.Data.FirstOrDefault(m => m.Aggregate == entity.Id);
                    if (aggregate != null)
                    {
                        if (entityMetrics == null)
                        {
                            entityMetrics = new List<AggregatedModel<int, Entity>>();
                        }
                        entityMetrics.Add(new AggregatedModel<int, Entity>(aggregate, entity));
                    }
                }
            }
            
            return entityMetrics?.OrderByDescending(o => o.Aggregate.Count) ?? null;
            
        }

        async Task<IEnumerable<AggregatedModel<int, User>>> SelectUsersByReputationAsync(DateTimeOffset start, DateTimeOffset end)
        {

            // Get views by id for specified range
            var viewsById = await _aggregatedUserReputationRepository.SelectSummedByInt("CreatedUserId", start, end);

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

            // No results to display
            List<AggregatedModel<int, User>> entityMetrics = null;
            if (mostViewedEntities?.Data != null)
            {
                // Build combined result
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
