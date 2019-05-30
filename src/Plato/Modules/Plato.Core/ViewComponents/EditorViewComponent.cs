using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Plato.Core.ViewModels;

namespace Plato.Core.ViewComponents
{
    public class EditorViewComponent : ViewComponent
    {

        public EditorViewComponent()
        {
         
        }

        public Task<IViewComponentResult> InvokeAsync(
            string id,
            string value,
            LocalizedHtmlString placeHolderText,
            string htmlName,
            bool autoFocus,
            int rows)
        {

            var model = new EditorViewModel
            {
                Id = id,
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText?.Value ?? string.Empty,
                Value = value,
                AutoFocus = autoFocus,
                Rows = rows > 0 ? rows : 10
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }

    }
    
}

