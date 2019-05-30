using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Layout.Titles;
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
            IFeatureFacade featureFacade, 
            IPageTitleBuilder pageTitleBuilder)
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
        
        public Task OnActionExecutingAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            
            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                return;
            }

            // Check early to ensure we are working with a LayoutViewModel
            if (!(result?.Model is LayoutViewModel model))
            {
                return;
            }

            // Get authenticated user from context
            var user = context.HttpContext.Features[typeof(User)] as User;

            // Get client details
            var ipV4Address = _clientIpAddress.GetIpV4Address();
            var ipV6Address = _clientIpAddress.GetIpV6Address();
            var userAgent = string.Empty;
            if (context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
            {
                userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
            }

            // Get route data
            var url = context.HttpContext.Request.GetDisplayUrl();

            // Get page title from context
            var title = context.HttpContext.Items[typeof(PageTitle)] is PageTitle pageTitle 
                ? pageTitle.Title 
                : string.Empty;
            
            // Get area name
            var areaName = string.Empty;
            if (context.RouteData.Values["area"] != null)
            {
                areaName = context.RouteData.Values["area"].ToString();
            }

            // Get feature from area
            var feature = await _featureFacade.GetFeatureByIdAsync(areaName);

            // Add metric
            await _metricManager.CreateAsync(new Metric()
            {
                FeatureId = feature?.Id ?? 0,
                Title = title,
                Url = url,
                IpV4Address = ipV4Address,
                IpV6Address = ipV6Address,
                UserAgent = userAgent,
                CreatedUserId = user?.Id ?? 0,
                CreatedDate = DateTimeOffset.UtcNow
            });

        }
    }

}
