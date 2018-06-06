using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Layout.Views
{

    public interface IGenericViewFactory
    {
        Task<GenericViewDescriptor> CreateAsync(string key, object view);

        Task<IHtmlContent> InvokeAsync(GenericViewDisplayContext displayContext);

        dynamic New { get; }
    }


    public class GenericViewFactory  : IGenericViewFactory
    {

        private readonly IGenericViewTableManager _genericViewTableManager;
        private readonly IGenericViewInvoker _genericViewInvoker;

        public GenericViewFactory(
            IGenericViewTableManager genericViewTableManager,
            IGenericViewInvoker genericViewInvoker)
        {
            _genericViewTableManager = genericViewTableManager;
            _genericViewInvoker = genericViewInvoker;
        }

        public async Task<GenericViewDescriptor> CreateAsync(string name, object model)
        {
            return await _genericViewTableManager.TryAdd(name, model);
        }

        public async Task<IHtmlContent> InvokeAsync(GenericViewDisplayContext displayContext)
        {

            // Contextulize generic view invoker

            _genericViewInvoker.Contextualize(displayContext);

            // Invoke generic view

            var htmlContent = await _genericViewInvoker.InvokeAsync(displayContext.ViewDescriptor);
            


            if (displayContext.ViewAdaptors != null)
            {
                foreach (var adaptorResult in displayContext.ViewAdaptors)
                {
                    var alterations = adaptorResult.AdaptorBuilder.CotentAlterations;
                    foreach (var alteration in alterations)
                    {
                        htmlContent = alteration(htmlContent);
                    }

                }
            }
          

            return htmlContent;

        }

        public dynamic New { get; }

        public IHtmlContent AlterInternal(IHtmlContent input, Func<IHtmlContent, IHtmlContent> action)
        {
            return action(input);
        }

    }
}
