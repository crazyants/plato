using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation.Abstractions;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout;
using Plato.Internal.Layout.Titles;
using Plato.Mentions.Services;

namespace Plato.Discuss.Mentions.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        public readonly IMentionTokenizer _mentionTokenzier;

        public HomeController(IMentionTokenizer mentionTokenzier)
        {
            _mentionTokenzier = mentionTokenzier;
        }
        
        public async Task<IActionResult> Index()
        {


            var text = @"Hi @admin,

            @admin, - @admin example @admin

            Hi @admin";
            
            var tokens = _mentionTokenzier.Tokenize(text);

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

            ViewData["test2"] = await _mentionTokenzier.ParseAsync(text);

            return View();

        }
    

    }

}
