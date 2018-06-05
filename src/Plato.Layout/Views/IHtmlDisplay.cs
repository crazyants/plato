using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Layout.Views
{

    public interface IHtmlDisplay
    {
        Task<IHtmlContent> ExecuteAsync(ViewDisplayContext context);
    }

    public class HtmlDisplay : IHtmlDisplay
    {
        public Task<IHtmlContent> ExecuteAsync(ViewDisplayContext context)
        {

          throw new NotImplementedException();

        }





    }
}
