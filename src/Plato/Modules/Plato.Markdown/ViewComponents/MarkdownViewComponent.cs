using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Markdown.Services;

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
            int id,
            string value,
            LocalizedHtmlString placeHolderText,
            string htmlName,
            bool autoFocus)
        {

            var model = new MarkdownViewModel
            {
                Id = id,
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText?.Value ?? string.Empty,
                Value = value,
                AutoFocusw = autoFocus
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }

    }
    
    public class MarkdownViewModel
    {

        public int Id { get; set; }

        public string Value { get; set; }

        public string PlaceHolderText { get; set; }

        public string HtmlName { get; set; }
        
        public bool AutoFocusw { get; set; }
        
    }

}

