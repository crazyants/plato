using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Plato.Internal.Layout.EmbeddedViews;
using Plato.Internal.Layout.Notifications;

namespace Plato.Internal.Layout.ViewHelpers
{
    public class NotifyViewHelper : EmbeddedView
    {

        private readonly Notification _notification;

        public NotifyViewHelper(Notification notification)
        {
            _notification = notification;
        }

        public override Task<IHtmlContent> Build()
        {
            if (_notification == null)
            {
                return Task.FromResult((IHtmlContent)HtmlString.Empty);
            }

            var builder = new HtmlContentBuilder();

            var htmlContentBuilder = builder
                .AppendHtml("<div class=\"alert")
                .AppendHtml(GetCssClass())
                .AppendHtml("\" role=\"alert\">")
                .AppendHtml(_notification.Message)
                .AppendHtml("</div>");

            return Task.FromResult((IHtmlContent)builder);

        }

        private string GetCssClass()
        {
            switch (_notification.Type)
            {
                case NotifyType.Success:
                    return " alert-success";
                case NotifyType.Info:
                    return " alert-info";
                case NotifyType.Warning:
                    return " alert-warning";
                case NotifyType.Danger:
                    return " alert-danger";
            }

            return string.Empty;

        }

    }
}
