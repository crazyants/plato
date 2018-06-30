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

            var markdown = @"# header 1

werwerwerw wer wer wer wrwr wr wrwerwer wr we rewrwerwerwerw wer wer wer wrwr wr wrwerwer wr we rewrwerwerwerw wer wer wer wrwr wr wrwerwer wr we rewr
werwerwerw wer wer wer wrwr wr wrwerwer wr we rewrwerwerwerw wer wer wer wrwr wr wrwerwer wr we rewrwerwerwerw wer wer wer wrwr wr wrwerwer wr we rewr

'''
werwerwerw wer wer wer wrwr wr wrwerwer wr we rewr
'''

## header 2

werwerwerw wer wer wer wrwr wr wrwerwer wr we rewr


";

            var parser = _markdownParserFactory.GetParser();
            var html = await parser.Parse(markdown);
            
            var model = new MarkdownViewModel
            {
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText,
                Value = value,
                Preview = html
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

