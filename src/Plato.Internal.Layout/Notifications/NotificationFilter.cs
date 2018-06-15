using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Plato.Internal.Models.Shell;

namespace Plato.Internal.Layout.Notifications
{
    public class NotificationFilter : IActionFilter, IAsyncResultFilter
    {

        public const string CookiePrefix = "plato_notify";


        private readonly ShellSettings _shellSettings;
        private readonly INotify _notify;
        private readonly ILogger<NotificationFilter> _logger;

        public NotificationFilter(
            ShellSettings shellSettings,
            INotify notify, 
            ILogger<NotificationFilter> logger)
        {
            _notify = notify;
            _shellSettings = shellSettings;
            _logger = logger;
        }


        #region "Implementation"

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
            var messages = Convert.ToString(context.HttpContext.Request.Cookies[CookiePrefix]);
            if (String.IsNullOrEmpty(messages))
            {
                return;
            }

            var notifications = DeserializeNotifications(messages);
            

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region "Private Methods"

        public IEnumerable<Notification> DeserializeNotifications(string messages)
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


            var output = "";
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

        #endregion






    }
}
