using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Layout.EmbeddedViews;

namespace Plato.Internal.Layout.ViewHelpers
{
    public class AlertViewHelper : EmbeddedView
    {

        readonly AlertInfo _alert;

        public AlertViewHelper(AlertInfo alert)
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
                .AppendHtml("<div class=\"alert alert-dismissible fade show")
                .AppendHtml(GetCssClass())
                .AppendHtml("\" role=\"alert\">")
                .AppendHtml(_alert.Message)
                .AppendHtml("<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">")
                .AppendHtml("<span aria-hidden=\"true\">&times;</span>")
                .AppendHtml("</button>")
                .AppendHtml("</div>");

            return Task.FromResult((IHtmlContent)builder);

        }

        string GetCssClass()
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
