using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Metrics.Models;
using Plato.Metrics.Services;

namespace Plato.Metrics.ActionFilters
{

    public class MetricFilter : IModularActionFilter
    {

        private readonly IMetricsManager<Metric> _metricManager;
        private readonly IClientIpAddress _clientIpAddress;

        public MetricFilter(
            IMetricsManager<Metric> metricManager,
            IClientIpAddress clientIpAddress)
        {
            _metricManager = metricManager;
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

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            
            // The controller action didn't return a view result so no need to continue execution
            var result = context.Result as ViewResult;
            if (result == null)
            {
                await next();
                return;
            }

            // Check early to ensure we are working with a LayoutViewModel
            var model = result.Model as LayoutViewModel;
            if (model == null)
            {
                await next();
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
            await _metricManager.CreateAsync(new Metric()
            {
                AreaName = context.RouteData.Values["area"].ToString(),
                ControllerName = context.RouteData.Values["controller"].ToString(),
                ActionName = context.RouteData.Values["action"].ToString(),
                IpV4Address = ipV4Address,
                IpV6Address = ipV6Address,
                UserAgent = userAgent,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

            // Finally execute the controller result
            await next();

        }

    }

}
