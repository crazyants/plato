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
using Plato.Layout.Display;
using Plato.Layout.Elements;

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

        public IHtmlContent Display(string sectionName, object model)
        {

            var htmlContentBuilder = new HtmlContentBuilder();

            var entries = _moduleManager.ModuleEntries;
            foreach (var entry in entries)
            {
                string id = entry.Descriptor.ID;
                                
                    ((IViewContextAware)_helper).Contextualize(this.ViewContext);
                    var descriptor = _selector.SelectComponent(id);

                    var component = DisplayElement(descriptor.FullName, model);

                    var writer = new System.IO.StringWriter();
                    component.WriteTo(writer, HtmlEncoder.Default);
                    htmlContentBuilder.AppendHtml("<div> XX ZONE 123 XX </div>");
                    htmlContentBuilder.AppendHtml(writer.ToString());
                             

            }

            IHtmlContent result = htmlContentBuilder;
            return result;

        }
        
        public async Task<IHtmlContent> DisplayAsync(string sectionName, object model)
        {
            var htmlContentBuilder = new HtmlContentBuilder();

            var entries = _moduleManager.ModuleEntries;
            foreach (var entry in entries)
            {
                string id = entry.Descriptor.ID;

                ((IViewContextAware)_helper).Contextualize(this.ViewContext);
                var descriptor = _selector.SelectComponent(id);

                var component = await _helper.InvokeAsync(id, model);

                //htmlContentBuilder.AppendHtml("<div>start</div><br>");
                htmlContentBuilder.AppendHtml(component);
                //htmlContentBuilder.AppendHtml("<div>end</div><br>");


            }
               
            return htmlContentBuilder;
        }

        public IHtmlContent DisplayElement(string fullName, object arguments)
        {
            Task<IHtmlContent> task = _helper.InvokeAsync(fullName, arguments);
            task.Wait();
            return task.Result;
        }
        



        //private bool Exists(string name)
        //{
        //    return _selector.SelectComponent(name) != null;
        //}

        //private bool Exists(Type componentType)
        //{
        //    var componentTypeInfo = componentType.GetTypeInfo();
        //    var descriptors = _descriptorProvider.ViewComponents;
        //    for (int i = 0; i < descriptors.Items.Count; i++)
        //    {
        //        var descriptor = descriptors.Items[i];
        //        if (descriptor.TypeInfo == componentTypeInfo)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private static string GetTypeForviewComponent(Type type, string extensionId)
        //{
        //    foreach (PlatoViewComponent featureAttribute in type.GetTypeInfo().GetCustomAttributes(typeof(PlatoViewComponent), false))
        //    {
        //        return featureAttribute.Name;
        //    }
        //    return extensionId;
        //}


    }
}
