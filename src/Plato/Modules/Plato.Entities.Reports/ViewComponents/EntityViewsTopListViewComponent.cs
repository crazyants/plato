using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Reports.ViewModels;
using Plato.Entities.Metrics.Repositories;
using Plato.Internal.Models.Metrics;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using System.Collections.Generic;
using System.Linq;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Reports.ViewComponents
{
    public class EntityViewsTopListViewComponent : ViewComponent
    {

        private readonly IAggregatedEntityMetricsRepository _aggregatedEntityMetricsRepository;
        private readonly IEntityStore<Entity> _entityStore;

        public EntityViewsTopListViewComponent(
            IAggregatedEntityMetricsRepository aggregatedEntityMetricsRepository,
            IEntityStore<Entity> entityStore)
        {
            _aggregatedEntityMetricsRepository = aggregatedEntityMetricsRepository;
            _entityStore = entityStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(ReportIndexOptions options)
        {
            
            if (options == null)
            {
                options = new ReportIndexOptions();
            }
            
            return View(await SelectEntitiesGroupedByViewsAsync(options.Start, options.End));

        }
        
        async Task<IEnumerable<AggregatedModel<int, Entity>>> SelectEntitiesGroupedByViewsAsync(DateTimeOffset start, DateTimeOffset end)
        {

            // Get views by grouped by entity id for specified range
            var viewsById = await _aggregatedEntityMetricsRepository.SelectGroupedByInt(
                "EntityId",
                start,
                end);

            // Get all entities matching ids
            IPagedResults<Entity> entities = null;
            if (viewsById != null)
            {
                entities = await _entityStore.QueryAsync()
                    .Take(1, 10)
                    .Select<EntityQueryParams>(q =>
                    {
                        q.Id.IsIn(viewsById.Data.Select(d => d.Aggregate).ToArray());
                    })
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
