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

    public class MarkdownController : Controller
    {
        private readonly IMarkdownParserFactory _markdownParserFactory;

        public MarkdownController(IMarkdownParserFactory markdownParserFactory)
        {
            _markdownParserFactory = markdownParserFactory;
        }

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Parse(string markdown)
        {
            if (String.IsNullOrEmpty(markdown))
            {
                return new ObjectResult(new
                {
                    string.Empty,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Album created successfully."
                });
            }

            var parser = _markdownParserFactory.GetParser();
            var html = await parser.ParseAsync(markdown);
            
            return new ObjectResult(new
            {
                html,
                StatusCode = HttpStatusCode.OK,
                Message = "Album created successfully."
            });

        }
    }
}