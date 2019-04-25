using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Entities.Metrics.Models;
using Plato.Entities.Metrics.Services;

namespace Plato.Entities.Metrics.ActionFilters
{

    public class EntityMetricFilter : IModularActionFilter
    {

        private readonly IEntityMetricsManager<EntityMetric> _entityMetricManager;
        private readonly IClientIpAddress _clientIpAddress;

        public EntityMetricFilter(
            IEntityMetricsManager<EntityMetric> entityMetricManager,
            IClientIpAddress clientIpAddress)
        {
            _entityMetricManager = entityMetricManager;
            _clientIpAddress = clientIpAddress;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }
        
        public Task OnActionExecutingAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnActionExecutedAsync(ResultExecutingContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            var result = context.Result as ViewResult;

            // Check early to ensure we are working with a LayoutViewModel
            var model = result?.Model as LayoutViewModel;
            if (model == null)
            {
                return;
            }

            // Check for the id route value 
            var id = context.RouteData.Values["opts.id"];
            if (id == null)
            {
                return;
            }

            // To int
            var ok = int.TryParse(id.ToString(), out var entityId);

            // Id route value is not a valid int
            if (!ok)
            {
                return;
            }

            // Get authenticated user from context
            var user = context.HttpContext.Features[typeof(User)] as User;

            // Get client details
            var ipV4Address = _clientIpAddress.GetIpV4Address();
            var ipV6Address = _clientIpAddress.GetIpV6Address();
            var userAgent = "";
            if (context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
            {
                userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            }

            // Add metric
            await _entityMetricManager.CreateAsync(new EntityMetric()
            {
                EntityId = entityId,
                IpV4Address = ipV4Address,
                IpV6Address = ipV6Address,
                UserAgent = userAgent,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

        }
    }

}
