using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Layout.Views
{
    public class DefaultHtmlDisplay : IHtmlDisplay
    {
        public Task<IHtmlContent> ExecuteAsync(GenericViewDisplayContext context)
        {
            throw new NotImplementedException();
        }
    }
}
