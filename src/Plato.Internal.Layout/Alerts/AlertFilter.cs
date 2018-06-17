using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Layout.ViewHelpers;
using Plato.Internal.Layout.Views;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Layout.Alerts
{
    public class AlertFilter : IActionFilter, IAsyncResultFilter
    {

        public const string CookiePrefix = "plato_notify";
        private bool _deleteCookie = false;
        readonly string _tenantPath;

        readonly ShellSettings _shellSettings;
        readonly ILayoutUpdater _layoutUpdater;
        readonly ILogger<AlertFilter> _logger;
        readonly IAlerter _alerter;

        public AlertFilter(
            ShellSettings shellSettings,
            ILogger<AlertFilter> logger,
            ILayoutUpdater layoutUpdater,
            IAlerter alerter)
        {
            _shellSettings = shellSettings;
            _logger = logger;
            _layoutUpdater = layoutUpdater;
            _alerter = alerter;
            _tenantPath = "/" + _shellSettings.RequestedUrlPrefix;
        }


        #region "Implementation"

        private ICollection<AlertInfo> _entries = new List<AlertInfo>();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
            var messages = Convert.ToString(context.HttpContext.Request.Cookies[CookiePrefix]);
            if (String.IsNullOrEmpty(messages))
            {
                return;
            }

            var notifications = DeserializeNotifications(messages);

            if (notifications == null)
            {
                _deleteCookie = true;
                return;
            }

            if (notifications.Count == 0)
            {
                return;
            }

            // Ensure notifications are available for the entire request
            _entries = notifications;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            var messageEntries = _alerter.Alerts;

            if (messageEntries == null)
            {
                return;
            }

            if (messageEntries.Count == 0 && _entries.Count == 0)
            {
                return;
            }

            // Assign values to the Items collection instead of TempData and
            // combine any existing entries added by the previous request with new ones.

            foreach (var entry in _entries)
            {
                messageEntries.Add(entry);
            }

            _entries = messageEntries;
            

            // Result is not a view, so assume a redirect and assign values to TemData.
            // String data type used instead of complex array to be session-friendly.
            if (!(context.Result is ViewResult) && _entries.Count > 0)
            {
                context.HttpContext.Response.Cookies.Append(CookiePrefix, SerializeNotifications(_entries),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = _tenantPath
                    });
            }

        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {


            if (_deleteCookie)
            {
                DeleteCookies(context);
                await next();
                return;
            }

            if (!(context.Result is ViewResult))
            {
                await next();
                return;
            }

            if (_entries.Count == 0)
            {
                await next();
                return;
            }

     
            var result = context.Result as ViewResult;
            if (result == null)
            {
                // The controller action didn't return a view result 
                // => no need to continue any further
                await next();
                return;
            }

            var model = result.Model as LayoutViewModel;
            if (model == null)
            {
                // there's no model or the model was not of the expected type 
                // => no need to continue any further
                await next();
                return;
            }


            if (context.Controller is Controller controller)
            {
                var updater = await _layoutUpdater.GetLayoutAsync(controller);
                await updater.UpdateLayoutAsync(async layout =>
                {
                    layout.Header = await updater.UpdateZoneAsync(layout.Header, zone =>
                    {
                        foreach (var entry in _entries)
                        {
                            zone.Add(new PositionedView(
                                new NotifyViewHelper(entry)
                            ));
                        }
                    });
                    
                    return layout;
                });
            }
            
            DeleteCookies(context);

            await next();
        }

        #endregion


        #region "Private Methods"

        public IList<AlertInfo> DeserializeNotifications(string messages)
        {

            List<AlertInfo> notifications;
            try
            {
                notifications = JsonConvert.DeserializeObject<List<AlertInfo>>(messages);
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the notifications
                _logger.LogError(e, e.Message);
                notifications = null;
            }

            return notifications;

        }

        public string SerializeNotifications(IEnumerable<AlertInfo> alert)
        {
            
            var output = string.Empty;
            try
            {
                output = JsonConvert.SerializeObject(alert);
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the notifications
                _logger.LogError(e, e.Message);
            }

            return output;

        }

        private void DeleteCookies(ResultExecutingContext context)
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
