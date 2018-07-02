using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IViewComponentResult> InvokeAsync(
            string value, 
            string placeHolderText,
            string htmlName)
        {

            var model = new MarkdownViewModel
            {
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText,
                Value = value,
            };

            return View(model);
        }

    }


    public class MarkdownViewModel
    {

        public string Value { get; set; }

        public string PlaceHolderText { get; set; }

        public string HtmlName { get; set; }


        public string Preview { get; set; }

    }

}

