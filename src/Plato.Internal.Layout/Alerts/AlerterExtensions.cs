using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Internal.Layout.Alerts
{
    public static class AlerterExtensions
    {

        public static void Success(this IAlerter alerter, LocalizedHtmlString message)
        {
            alerter.Add(AlertType.Success, message);
        }

        public static void Info(this IAlerter alerter, LocalizedHtmlString message)
        {
            alerter.Add(AlertType.Info, message);
        }
        public static void Warning(this IAlerter alerter, LocalizedHtmlString message)
        {
            alerter.Add(AlertType.Warning, message);
        }

        public static void Danger(this IAlerter alerter, LocalizedHtmlString message)
        {
            alerter.Add(AlertType.Danger, message);
        }
        
    }

}
