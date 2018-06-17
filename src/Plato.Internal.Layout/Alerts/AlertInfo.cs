using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Internal.Layout.Alerts
{

    public enum AlertType
    {
        Success,
        Info,
        Warning,
        Danger
    }
    
    public class AlertInfo
    {

        public AlertType Type { get; set; }

        public LocalizedHtmlString Message { get; set; }

        public AlertInfo(AlertType type, LocalizedHtmlString message)
        {
            this.Type = type;
            this.Message = message;
        }

    }
}
