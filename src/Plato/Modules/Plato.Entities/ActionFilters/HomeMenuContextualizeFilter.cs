using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ActionFilters;

namespace Plato.Entities.ActionFilters
{

    public class HomeMenuContextualizeFilter : IModularActionFilter
    {

        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;

        public HomeMenuContextualizeFilter(
            IAggregatedEntityRepository aggregatedEntityRepository)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                return;
            }
            
            // Check early to ensure we are working with a LayoutViewModel
            if (!(result.Model is LayoutViewModel model))
            {
                return;
            }

            // Plato.Core?
            var isArea = context.RouteData.Values["area"].ToString()
                .Equals("Plato.Core", StringComparison.OrdinalIgnoreCase);

            if (!isArea)
            {
                return;
            }
            
            // Home controller?
            var isController = context.RouteData.Values["controller"].ToString()
                .Equals("Home", StringComparison.OrdinalIgnoreCase);

            if (!isController)
            {
                return;
            }

            // Index action?
            var isAction = context.RouteData.Values["action"].ToString()
                .Equals("Index", StringComparison.OrdinalIgnoreCase);
            
            if (!isAction)
            {
                return;
            }

            // We are on the homepage, register metrics on context
            context.HttpContext.Items[typeof(FeatureEntityMetrics)] = new FeatureEntityMetrics()
            {
                Metrics = await _aggregatedEntityRepository.SelectGroupedByFeatureAsync()
            };

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }
    }

}
