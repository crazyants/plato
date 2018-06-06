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
            _genericViewInvoker.Contextualize(displayContext);
            return await _genericViewInvoker.InvokeAsync(displayContext.ViewDescriptor);
        }

        public dynamic New { get; }

    }
}
