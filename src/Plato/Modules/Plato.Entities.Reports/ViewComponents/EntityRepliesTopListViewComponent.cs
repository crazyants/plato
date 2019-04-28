using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Reports.ViewModels;
using Plato.Internal.Models.Metrics;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using System.Collections.Generic;
using System.Linq;
using Plato.Entities.Repositories;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Reports.ViewComponents
{
    public class EntityRepliesTopListViewComponent : ViewComponent
    {

        private readonly IAggregatedEntityReplyRepository _aggregatedEntityReplyRepository;
        private readonly IEntityStore<Entity> _entityStore;

        public EntityRepliesTopListViewComponent(
            IAggregatedEntityReplyRepository aggregatedEntityReplyRepository,
            IEntityStore<Entity> entityStore)
        {
            _aggregatedEntityReplyRepository = aggregatedEntityReplyRepository;
            _entityStore = entityStore;
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
            
            return View(new ChartViewModel<IEnumerable<AggregatedModel<int, Entity>>>()
            {
                Options = chart,
                Data = await SelectEntitiesGroupedByViewsAsync(options)
            });

        }

        async Task<IEnumerable<AggregatedModel<int, Entity>>> SelectEntitiesGroupedByViewsAsync(ReportOptions options)
        {

            // Get views by grouped by entity id for specified range
            var viewsById = options.FeatureId > 0
                ? await _aggregatedEntityReplyRepository.SelectGroupedByIntAsync(
                    "EntityId",
                    options.Start,
                    options.End,
                    options.FeatureId)
                : await _aggregatedEntityReplyRepository.SelectGroupedByIntAsync(
                    "EntityId",
                    options.Start,
                    options.End);

            // Get all entities matching ids
            IPagedResults<Entity> entities = null;
            if (viewsById != null)
            {
                entities = await _entityStore.QueryAsync()
                    .Take(1, 10)
                    .Select<EntityQueryParams>(q => { q.Id.IsIn(viewsById.Data.Select(d => d.Aggregate).ToArray()); })
                    .OrderBy("CreatedDate", OrderBy.Desc)
                    .ToList();
            }

            // Build aggregated list
            List<AggregatedModel<int, Entity>> metrics = null;
            if (entities?.Data != null)
            {
                foreach (var entity in entities.Data)
                {
                    // Get or add aggregate
                    var aggregate = viewsById?.Data.FirstOrDefault(m => m.Aggregate == entity.Id);
                    if (aggregate != null)
                    {
                        if (metrics == null)
                        {
                            metrics = new List<AggregatedModel<int, Entity>>();
                        }

                        metrics.Add(new AggregatedModel<int, Entity>(aggregate, entity));
                    }
                }
            }

            return metrics?.OrderByDescending(o => o.Aggregate.Count) ?? null;

        }

    }

}
