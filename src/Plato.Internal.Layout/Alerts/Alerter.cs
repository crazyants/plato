using Microsoft.AspNetCore.Mvc.Localization;
using System.Collections.Generic;

namespace Plato.Internal.Layout.Alerts
{
 
    public interface IAlerter
    {

        void Add(AlertType type, LocalizedHtmlString message);

        ICollection<AlertInfo> Alerts { get; }

    }

    public class Alerter : IAlerter
    {

        public ICollection<AlertInfo> Alerts { get; set; }

        public void Add(AlertType type, LocalizedHtmlString message)
        {
            if (Alerts == null)
            {
                Alerts = new List<AlertInfo>();
            }
            Alerts.Add(new AlertInfo(type, message));
        }

    }
}
