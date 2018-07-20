using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Plato.Core.ViewComponents
{
    public class EditorViewComponent : ViewComponent
    {

        public EditorViewComponent()
        {
         
        }

        public Task<IViewComponentResult> InvokeAsync(
            int id,
            string value,
            LocalizedHtmlString placeHolderText,
            string htmlName,
            bool autoFocus)
        {

            var model = new EditorViewModel
            {
                Id = id,
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText?.Value ?? string.Empty,
                Value = value,
                AutoFocus = autoFocus
            };

            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }

    public class EditorViewModel
    {

        public int Id { get; set; }

        public string Value { get; set; }

        public string PlaceHolderText { get; set; }

        public string HtmlName { get; set; }

        public bool AutoFocus { get; set; }

    }

}

