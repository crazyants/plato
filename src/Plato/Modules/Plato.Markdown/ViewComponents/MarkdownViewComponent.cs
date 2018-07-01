using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Markdown.Services;

namespace Plato.Markdown.ViewComponents
{
    public class MarkdownViewComponent : ViewComponent
    {

        private readonly IMarkdownSubscriber _markdownSubscriber;
        private readonly IMarkdownParserFactory _markdownParserFactory;

        public MarkdownViewComponent(
            IMarkdownParserFactory markdownParserFactory,
            IMarkdownSubscriber markdownSubscriber)
        {
            _markdownParserFactory = markdownParserFactory;
            _markdownSubscriber = markdownSubscriber;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string value, 
            string placeHolderText,
            string htmlName)
        {

            // add a subscription to our message broker
            // this subscription is used by 
            _markdownSubscriber.Subscribe();

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

