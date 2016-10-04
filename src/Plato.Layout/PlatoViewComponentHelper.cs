using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Plato.Layout
{
    public class PlatoViewComponentHelper : DefaultViewComponentHelper
    {

        private readonly IViewComponentDescriptorCollectionProvider _descriptorProvider;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly IViewComponentInvokerFactory _invokerFactory;
        private readonly IViewComponentSelector _selector;
        private readonly IViewBufferScope _viewBufferScope;
    
        public PlatoViewComponentHelper(
            IViewComponentDescriptorCollectionProvider descriptorProvider,
            HtmlEncoder htmlEncoder,
            IViewComponentSelector selector,
            IViewComponentInvokerFactory factory,
            IViewBufferScope viewBufferScope)
        : base(descriptorProvider, htmlEncoder, selector, factory, viewBufferScope)
        {
            _descriptorProvider = descriptorProvider;
            _htmlEncoder = htmlEncoder;
            _selector = selector;        
            _viewBufferScope = viewBufferScope;
        }

        //public Task<IHtmlContent> InvokeAsync(string name, object arguments)
        //{
        //    if (name == null)
        //    {
        //        throw new ArgumentNullException(nameof(name));
        //    }

        //    var descriptor = _selector.SelectComponent(name);
        //    if (descriptor == null)
        //    {
        //        throw new InvalidOperationException(Resources.FormatViewComponent_CannotFindComponent(name));
        //    }

        //    return InvokeCoreAsync(descriptor, arguments);
        //}

    }
}
