using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Plato.Internal.Layout.EmbeddedViews;

namespace Plato.Internal.Layout.ViewHelpers
{

    public class HtmlViewHelper : EmbeddedView
    {

        private readonly string _html;

        public HtmlViewHelper(string html)
        {
            _html = html;
        }
        
        public override Task<IHtmlContent> Build()
        {
            return Task.FromResult((IHtmlContent)new HtmlString(_html));
        }

    }
}
