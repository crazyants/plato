using Microsoft.AspNetCore.Mvc.Localization;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Plato.Internal.Layout.Alerts
{
 
    public interface IAlerter
    {

        void Add(AlertType type, LocalizedHtmlString message);

        IList<AlertInfo> Alerts();

    }

    public class Alerter : IAlerter
    {

        private readonly IList<AlertInfo> _alerts;

        public IList<AlertInfo> Alerts()
        {
            return _alerts;
        }

        private readonly ILogger<Alerter> _logger;

        public Alerter(ILogger<Alerter> logger)
        {
            _logger = logger;
            _alerts = new List<AlertInfo>();
        }

        public void Add(AlertType type, LocalizedHtmlString message)
        {
            _logger.LogInformation($"The alert of type '{type.ToString()}' was added for display. Message: '{message}'.");
            _alerts.Add(new AlertInfo(type, message));
        }

    }
}
