using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.EmbeddedViews;

namespace Plato.Internal.Layout.ViewHelpers
{
    public class NotifyViewHelper : EmbeddedView
    {

        private readonly AlertInfo _alert;

        public NotifyViewHelper(AlertInfo alert)
        {
            _alert = alert;
        }

        public override Task<IHtmlContent> Build()
        {
            if (_alert == null)
            {
                return Task.FromResult((IHtmlContent)HtmlString.Empty);
            }

            var builder = new HtmlContentBuilder();

            var htmlContentBuilder = builder
                .AppendHtml("<div class=\"alert")
                .AppendHtml(GetCssClass())
                .AppendHtml("\" role=\"alert\">")
                .AppendHtml(_alert.Message)
                .AppendHtml("</div>");

            return Task.FromResult((IHtmlContent)builder);

        }

        private string GetCssClass()
        {
            switch (_alert.Type)
            {
                case AlertType.Success:
                    return " alert-success";
                case AlertType.Info:
                    return " alert-info";
                case AlertType.Warning:
                    return " alert-warning";
                case AlertType.Danger:
                    return " alert-danger";
            }

            return string.Empty;

        }

    }
}
