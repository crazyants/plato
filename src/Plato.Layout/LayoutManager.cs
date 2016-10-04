using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using Plato.Environment.Modules.Abstractions;
using System.Reflection;
using Plato.Abstractions.Layout;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Layout
{
    public class LayoutManager : ILayoutManager
    {

        IViewComponentSelector _selector;
        IViewComponentHelper _helper;
        IViewComponentInvokerFactory _factory;
        IModuleManager _moduleManager;
        IViewComponentDescriptorCollectionProvider _descriptorProvider;
        IViewBufferScope _viewBufferScope;

        public LayoutManager(
            IViewComponentDescriptorCollectionProvider descriptorProvider,
            IViewComponentSelector selector,
            IViewComponentInvokerFactory factory,
            IViewComponentHelper helper,
            IModuleManager moduleManager,
            IViewBufferScope viewBufferScope)
        {
            _descriptorProvider = descriptorProvider;
            _selector = selector;
            _factory = factory;
            _helper = helper;
            _moduleManager = moduleManager;
            _viewBufferScope = viewBufferScope;
        }
        
        [ViewContext]
        public ViewContext ViewContext { get; set;  }

        public IHtmlContent Display(string sectionName, object arguments)
        {

            var htmlContentBuilder = new HtmlContentBuilder();

            var entries = _moduleManager.ModuleEntries;
            foreach (var entry in entries)
            {

                //var helper = new DefaultViewComponentHelper(_selector, _factory);

                ((IViewContextAware)_helper).Contextualize(this.ViewContext);
                var descriptor = _selector.SelectComponent(entry.Descriptor.ID);
              
                Task<IHtmlContent> task = _helper.InvokeAsync(descriptor.FullName, arguments);              
                task.Wait();

                var writer = new System.IO.StringWriter();
                task.Result.WriteTo(writer, HtmlEncoder.Default);
                htmlContentBuilder.AppendHtml("<div> XX ZONE 123 XX </div>");
                htmlContentBuilder.AppendHtml(writer.ToString());


            }

            IHtmlContent result = htmlContentBuilder;
            return result;

        }





        private bool Exists(string name)
        {
            return _selector.SelectComponent(name) != null;
        }

        private bool Exists(Type componentType)
        {
            var componentTypeInfo = componentType.GetTypeInfo();
            var descriptors = _descriptorProvider.ViewComponents;
            for (int i = 0; i < descriptors.Items.Count; i++)
            {
                var descriptor = descriptors.Items[i];
                if (descriptor.TypeInfo == componentTypeInfo)
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetTypeForviewComponent(Type type, string extensionId)
        {
            foreach (PlatoViewComponent featureAttribute in type.GetTypeInfo().GetCustomAttributes(typeof(PlatoViewComponent), false))
            {
                return featureAttribute.Name;
            }
            return extensionId;
        }


    }
}
