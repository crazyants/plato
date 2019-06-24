using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Layout.ModelBinding;
using Plato.Mentions.Services;
using Plato.Internal.Abstractions.Extensions;

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


            var text = @"Hi @admin

            Hi @admin,

Need your help. Just testing @mentions

@admin@123@emailEmAil52155571.com @admin 

Hi @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin, @admin,

#333, @admin, #315, #316 
Hi @admin 

@emailEmAil52155571.com,@emailEmAil17858828.com,@emailEmAil44877050.com @123 @AlDennis @atpw25, @appleskin  @atpw25, @atpw25,  @atpw25 

@appleskin 

@appleskin 

@ToadRage @TedProsoft @tim @typedef @ToadRage  @thechop 

Can you help 

@admin ";
            
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
            }

            //ViewData["test1"] = sb.ToString();

            ViewData["test1"] = await _parser.ParseAsync(text);

            var html = text.NormalizeNewLines();
            html = html.Replace("&#xA;", "\n");
            html = html.HtmlTextulize();

            ViewData["test2"] = await _parser.ParseAsync(html);

            return View();

        }
    

    }

}
