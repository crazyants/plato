using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Features.Abstractions;
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
        private readonly IFeatureFacade _featureFacade;

        public MetricFilter(
            IMetricsManager<Metric> metricManager,
            IClientIpAddress clientIpAddress, 
            IFeatureFacade featureFacade)
        {
            _metricManager = metricManager;
            _clientIpAddress = clientIpAddress;
            _featureFacade = featureFacade;
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

            // Check early to ensure we are working with a LayoutViewModel
            var model = result?.Model as LayoutViewModel;
            if (model == null)
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

            // Get route data

            var areaName = "";
            if (context.RouteData.Values["area"] != null)
            {
                areaName = context.RouteData.Values["area"].ToString();
            }

            var controllerName = "";
            if (context.RouteData.Values["controller"] != null)
            {
                controllerName = context.RouteData.Values["controller"].ToString();
            }

            var actionName = "";
            if (context.RouteData.Values["action"] != null)
            {
                actionName = context.RouteData.Values["action"].ToString();
            }

            // Get feature from area
            var feature = await _featureFacade.GetFeatureByIdAsync(areaName);
            
            // Add metric
            await _metricManager.CreateAsync(new Metric()
            {
                FeatureId = feature?.Id ?? 0,
                AreaName = areaName,
                ControllerName = controllerName,
                ActionName = actionName,
                IpV4Address = ipV4Address,
                IpV6Address = ipV6Address,
                UserAgent = userAgent,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });


        }

    }

}
