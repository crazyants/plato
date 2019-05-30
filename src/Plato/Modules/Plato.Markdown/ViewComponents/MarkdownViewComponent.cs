using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Markdown.Services;
using Plato.Markdown.ViewModels;

namespace Plato.Markdown.ViewComponents
{
    public class MarkdownViewComponent : ViewComponent
    {
     
        private readonly IMarkdownParserFactory _markdownParserFactory;

        public MarkdownViewComponent(
            IMarkdownParserFactory markdownParserFactory)
        {
            _markdownParserFactory = markdownParserFactory;
        }

        public Task<IViewComponentResult> InvokeAsync(
            string id,
            string value,
            LocalizedHtmlString placeHolderText,
            string htmlName,
            bool autoFocus,
            int tabIndex,
            int rows)
        {

            var model = new MarkdownViewModel
            {
                Id = id,
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText?.Value ?? string.Empty,
                Value = value,
                AutoFocus = autoFocus,
                TabIndex = tabIndex,
                Rows = rows > 0 ? rows : 10
            };

            return Task.FromResult((IViewComponentResult)View(model));

        }

    }
    
}

