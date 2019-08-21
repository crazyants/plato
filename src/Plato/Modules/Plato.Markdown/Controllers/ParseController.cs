using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Messaging.Abstractions;
using Plato.WebApi.Controllers;

namespace Plato.Markdown.Controllers
{

    public class MarkDownInput
    {
        public string Markdown { get; set; }
    }
    
    public class ParseController : BaseWebApiController
    {
  
        private readonly IBroker _broker;

        public ParseController(IBroker broker)
        {
            _broker = broker;
        }

        [HttpPost, ResponseCache(NoStore = true)]
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

            // Parse Html via ParseEntityHtml message broker subscriptions
            var html = markdownInput.Markdown;
            foreach (var handler in _broker.Pub<string>(this, "ParseEntityHtml"))
            {
                html = await handler.Invoke(new Message<string>(html, this));
            }
            
            return base.Result(html);

        }

    }
}