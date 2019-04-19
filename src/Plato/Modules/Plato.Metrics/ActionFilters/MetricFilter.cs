using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Users;
using Plato.Metrics.Models;
using Plato.Metrics.Services;

namespace Plato.Metrics.ActionFilters
{

    public class MetricFilter : IModularActionFilter
    {

        private readonly IMetricsManager<Metric> _metricManager;
       
        public MetricFilter(IMetricsManager<Metric> metricManager)
        {
            _metricManager = metricManager;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            // Get authenticated user from context
            var user = context.HttpContext.Features[typeof(User)] as User;

            var routeData = context.RouteData;
            var values = 

            await _metricManager.CreateAsync(new Metric()
            {
                AreaName = context.RouteData.Values["area"].ToString(),
                ControllerName = context.RouteData.Values["controller"].ToString(),
                ActionName = context.RouteData.Values["action"].ToString(),
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

            // Finally execute the controller result
            await next();

        }
    }
}
