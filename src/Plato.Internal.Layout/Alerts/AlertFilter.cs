using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Layout.ViewHelpers;
using Plato.Internal.Layout.Views;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Layout.Alerts
{
    public class AlertFilter : IActionFilter, IAsyncResultFilter
    {

        internal const string CookiePrefix = "plato_alerts";
        private bool _deleteCookie = false;
        readonly string _tenantPath;
        private IList<AlertInfo> _existingAlerts = new List<AlertInfo>();

        private readonly HtmlEncoder _htmlEncoder;
        readonly ILayoutUpdater _layoutUpdater;
        readonly ILogger<AlertFilter> _logger;
        readonly IAlerter _alerter;

        public AlertFilter(
            ShellSettings shellSettings,
            HtmlEncoder htmlEncoder,
            ILogger<AlertFilter> logger,
            ILayoutUpdater layoutUpdater,
            IAlerter alerter)
        {
            _layoutUpdater = layoutUpdater;
            _alerter = alerter;
            _htmlEncoder = htmlEncoder;
            _logger = logger;
            _tenantPath = "/" + shellSettings.RequestedUrlPrefix;
        }

        #region "Filter Implementation"

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
            // 1.
            var json = Convert.ToString(
                context.HttpContext.Request.Cookies[CookiePrefix]
            );

            if (String.IsNullOrEmpty(json))
            {
                return;
            }

            // Deserialize alerts store
            var alerts = DeserializeAlerts(json);
            if (alerts == null)
            {
                _deleteCookie = true;
                return;
            }

            if (alerts.Count == 0)
            {
                return;
            }

            // Ensure alerts are available for the entire request
            _existingAlerts = alerts;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            // 2.
            var alerts = _alerter.Alerts();

            if (alerts == null)
            {
                return;
            }

            if (alerts.Count == 0 && _existingAlerts.Count == 0)
            {
                return;
            }
            
            // Persist alerts for the entire request so they are available
            // for display within OnResultExecutionAsync below
            foreach (var alert in alerts)
            {
                _existingAlerts.Add(alert);
            }
            
            // Result is not a view, so assume a redirect and assign values to persistance
            if (!(context.Result is ViewResult) && _existingAlerts.Count > 0)
            {
                context.HttpContext.Response.Cookies.Append(
                    CookiePrefix,
                    SerializeAlerts(_existingAlerts),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = _tenantPath
                    });
            }

        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            
            // 3.
            if (_deleteCookie)
            {
                DeleteCookies(context);
                await next();
                return;
            }
            
            // We don't have any alerts
            if (_existingAlerts.Count == 0)
            {
                await next();
                return;
            }

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

            // Add alerts to layout view model
            if (context.Controller is Controller controller)
            {
                var updater = await _layoutUpdater.GetLayoutAsync(controller);
                await updater.UpdateLayoutAsync(async layout =>
                {
                    layout.Alerts = await updater.UpdateZoneAsync(layout.Alerts, zone =>
                    {
                        foreach (var alert in _existingAlerts)
                        {
                            zone.Add(new PositionedView(new AlertViewHelper(alert)));
                        }
                    });
                    
                    return layout;
                });
            }
            
            // We've displayed our alert so delete persistance
            // to ensure no further alerts are displayed
            DeleteCookies(context);

            // Finally execute the controller result
            await next();

        }

        #endregion
        
        #region "Private Methods"

        IList<AlertInfo> DeserializeAlerts(string messages)
        {
         
            List<AlertInfo> alerts;
            try
            {
                alerts = JsonConvert.DeserializeObject<List<AlertInfo>>(messages, JsonSettings());
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the alerts
                // Return null to ensure _deleteCookie is set to true
                // and persistance is deleted within OnResultExecutionAsync
                _logger.LogError(e, e.Message);
                alerts = null;
            }

            return alerts;

        }

        string SerializeAlerts(IList<AlertInfo> alert)
        {
            
            var output = string.Empty;
            try
            {
                output = JsonConvert.SerializeObject(alert, JsonSettings());
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the alerts
                _logger.LogError(e, e.Message);
            }

            return output;

        }

        JsonSerializerSettings JsonSettings()
        {
            return new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                    new AlertInfoConverter(_htmlEncoder)
                }
            };
        }

        void DeleteCookies(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Delete(
                CookiePrefix,
                new CookieOptions
                {
                    Path = _tenantPath
                });
        }
        
        #endregion
        
    }
}
