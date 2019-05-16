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
using Microsoft.AspNetCore.Antiforgery.Internal;
using Plato.Internal.Data.Abstractions;

namespace Plato.Entities.Reports.ViewComponents
{
    public class NewEntitiesListViewComponent : ViewComponent
    {


        private readonly IEntityStore<Entity> _entityStore;

        public NewEntitiesListViewComponent(
            IEntityStore<Entity> entityStore)
        {
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

            var data = await _entityStore.QueryAsync()
                .Take(1, 10)
                .Select<EntityQueryParams>(q =>
                {
                    q.CreatedDate.GreaterThan(options.Start);
                    q.CreatedDate.LessThanOrEqual(options.End);
                })
                .OrderBy("CreatedDate", OrderBy.Desc)
                .ToList();

            return View(new ChartViewModel<IPagedResults<Entity>>()
            {
                Options = chart,
                Data = data
            });

        }


    }

}
