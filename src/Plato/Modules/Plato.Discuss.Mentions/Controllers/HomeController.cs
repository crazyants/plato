using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Layout.ModelBinding;
using Plato.Mentions.Services;

namespace Plato.Discuss.Mentions.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        public readonly IMentionsParser _parser;
        public readonly IMentionsTokenizer _tokenzier;

        public HomeController(
            IMentionsParser mentionsParser,
            IMentionsTokenizer mentionTokenzier)
        {
            _tokenzier = mentionTokenzier;
            _parser = mentionsParser;
        }
        
        public async Task<IActionResult> Index()
        {


            var text = @"Hi @admin,

            @admin, - @admin example @admin

            Hi @admin";
            
            var tokens = _tokenzier.Tokenize(text);

            var sb = new System.Text.StringBuilder();
            foreach (var t in tokens)
            {
                sb.Append("start: ").Append(t.Start);
                sb.Append("<br>");
                sb.Append("end: ").Append(t.End);
                sb.Append("<br>");
                sb.Append("value: ").Append(t.Value);
                sb.Append("<br>");
                sb.Append("<br>");
            }

            ViewData["test1"] = sb.ToString();

            ViewData["test2"] = await _parser.ParseAsync(text);

            return View();

        }
    

    }

}
