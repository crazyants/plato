using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Layout.Elements;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

namespace Plato.Layout.Display
{
    public class DisplayHelper
    {

        public ViewContext ViewContext { get; set; }


        public async Task<IEnumerable<IHtmlContent>> ElementExecuteAsync(IEnumerable<object> shapes)
        {
            var result = new List<IHtmlContent>();
            foreach (var shape in shapes)
            {
                result.Add(await ShapeExecuteAsync(shape));
            }

            return result;
        }
        public async Task<IHtmlContent> ShapeExecuteAsync(object shape)
        {
            if (shape == null)
            {
                return new HtmlString(string.Empty);
            }

            var context = new DisplayContext { Display = this, Value = shape, ViewContext = ViewContext };
            return await ExecuteAsync(context);
        }


        public async Task<IHtmlContent> ExecuteAsync(DisplayContext context)
        {
            var element = context.Value as IElement;

            if (element == null)
                return CoerceHtmlString(context.Value);







            return element.MetaData.ChildContent;

            //return await Task.FromResult(new HtmlString(""));


        }


        static IHtmlContent CoerceHtmlString(object value)
        {
            if (value == null)
                return null;

            var result = value as IHtmlContent;
            if (result != null)
                return result;

            return new HtmlString(HtmlEncoder.Default.Encode(value.ToString()));
        }


    }
}
