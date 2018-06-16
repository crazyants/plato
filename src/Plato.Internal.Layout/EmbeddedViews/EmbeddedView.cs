using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Internal.Layout.EmbeddedViews
{
    public abstract class EmbeddedView : IEmbeddedView
    {

        private ViewContext _context;

        public ViewContext ViewContext
        {
            get
            {
                if (_context == null)
                {
                    throw new Exception($"The view context has not been initialized within embedded view '{this.GetType().ToString()}'. The ViewContext is not accessible within the constructor and is only available once the Build() method within your embedded view has been called.");
                }
                return _context;
            }
        } 

        public abstract Task<IHtmlContent> Build();
        
        public IEmbeddedView Contextualize(ViewContext context)
        {
            _context = context;
            return this;
        }

    }
}
