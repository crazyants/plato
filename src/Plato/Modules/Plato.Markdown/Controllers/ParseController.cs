using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Markdown.Services;

namespace Plato.Markdown.Controllers
{


    public class MarkDownInput
    {
        public string Markdown { get; set; }
    }


    public class ParseController : Controller
    {
        private readonly IMarkdownParserFactory _markdownParserFactory;

        public ParseController(IMarkdownParserFactory markdownParserFactory)
        {
            _markdownParserFactory = markdownParserFactory;
        }

        [HttpPost]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Post([FromBody] MarkDownInput markdownInput)
        {
            if (String.IsNullOrEmpty(markdownInput.Markdown))
            {
                return new ObjectResult(new
                {
                    string.Empty,
                    StatusCode = HttpStatusCode.OK,
                    Message = "No markdown input was supplied"
                });
            }

            var parser = _markdownParserFactory.GetParser();
            var html = await parser.ParseAsync(markdownInput.Markdown);
            
            return new ObjectResult(new
            {
                html,
                StatusCode = HttpStatusCode.OK,
                Message = "Markdown parsed successfully"
            });

        }
    }
}