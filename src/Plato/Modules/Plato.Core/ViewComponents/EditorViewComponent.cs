using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Core.ViewComponents
{
    public class EditorViewComponent : ViewComponent
    {
      

        public EditorViewComponent()
        {
         
        }

        public Task<IViewComponentResult> InvokeAsync(
            string value, 
            string placeHolderText,
            string htmlName)
        {

            var model = new EditorViewModel
            {
                HtmlName = htmlName,
                PlaceHolderText = placeHolderText,
                Value = value
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

        public string Value { get; set; }

        public string PlaceHolderText { get; set; }

        public string HtmlName { get; set; }

    }

}

