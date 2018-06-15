using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Layout.Notifications
{
    public class NotificationFilter : IActionFilter, IAsyncResultFilter
    {

        public const string CookiePrefix = "plato_notify";
        private bool _deleteCookie = false;
        readonly string _tenantPath;

        readonly ShellSettings _shellSettings;
        readonly ILayoutAccessor _layoutAccessor;
        readonly ILogger<NotificationFilter> _logger;
        readonly INotify _notify;

        public NotificationFilter(
            ShellSettings shellSettings,
            ILogger<NotificationFilter> logger, 
            ILayoutAccessor layoutAccessor,
            INotify notify)
        {
         
            _shellSettings = shellSettings;
            _logger = logger;
            _layoutAccessor = layoutAccessor;
            _notify = notify;

            _tenantPath = "/" + _shellSettings.RequestedUrlPrefix;
        }


        #region "Implementation"

        private ICollection<Notification> _existingEntries = new List<Notification>();

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
            _existingEntries = notifications;

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            var messageEntries = _notify.Notifications;

            if (messageEntries == null)
            {
                return;
            }

            if (messageEntries.Count == 0 && _existingEntries.Count == 0)
            {
                return;
            }

            // Assign values to the Items collection instead of TempData and
            // combine any existing entries added by the previous request with new ones.

            foreach (var entry in _existingEntries)
            {
                messageEntries.Add(entry);
            }

            _existingEntries = messageEntries;


            // Result is not a view, so assume a redirect and assign values to TemData.
            // String data type used instead of complex array to be session-friendly.
            if (!(context.Result is ViewResult) && _existingEntries.Count > 0)
            {
                context.HttpContext.Response.Cookies.Append(CookiePrefix, SerializeNotifications(_existingEntries),
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

            if (_existingEntries.Count == 0)
            {
                await next();
                return;
            }
            
            var keys = context.ModelState.Keys;

            var layout = await _layoutAccessor.GetLayoutAsync();

         

            //dynamic layout = await _layoutAccessor.GetLayoutAsync();
            //var messagesZone = layout.Zones["Messages"];

            //foreach (var messageEntry in _existingEntries)
            //{
            //    messagesZone = messagesZone.Add(await _shapeFactory.Message(messageEntry));
            //}

            DeleteCookies(context);

            await next();
        }

        #endregion


        #region "Private Methods"

        public IList<Notification> DeserializeNotifications(string messages)
        {

            List<Notification> notifications;
            try
            {
                notifications = JsonConvert.DeserializeObject<List<Notification>>(messages);
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the notifications
                _logger.LogError(e, e.Message);
                notifications = null;
            }

            return notifications;

        }

        public string SerializeNotifications(IEnumerable<Notification> notifications)
        {
            
            var output = string.Empty;
            try
            {
                output = JsonConvert.SerializeObject(notifications);
            }
            catch (Exception e)
            {
                // A problem occurring deserializing the notifications
                _logger.LogError(e, e.Message);
                notifications = null;
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
