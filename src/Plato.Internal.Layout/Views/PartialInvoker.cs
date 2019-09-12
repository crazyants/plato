using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Plato.Internal.Abstractions;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Plato.Internal.Layout.Views;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using Microsoft.DotNet.PlatformAbstractions;
using Plato.Internal.Cache.Abstractions;
using System.Text.Encodings.Web;

namespace Plato.Internal.Layout.Views
{


    public interface IPartialInvoker
    {

        ViewContext ViewContext { get; set; }

        void Contextualize(ViewContext viewContext);

        Task<IHtmlContent> InvokeAsync(string viewName, object model, ViewDataDictionary viewData);
    }

    public class PartialInvoker : IPartialInvoker
    {

        private readonly IMemoryCache _memoryCache;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IViewBufferScope _viewBufferScope;
        
        public PartialInvoker(
            IMemoryCache memoryCache,
            ICompositeViewEngine viewEngine,
            IViewBufferScope viewBufferScope)
        {
            _memoryCache = memoryCache;
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            _viewBufferScope = viewBufferScope ?? throw new ArgumentNullException(nameof(viewBufferScope));            
        }
        
        public ViewContext ViewContext { get; set; }

        public void Contextualize(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(string viewName, object model, ViewDataDictionary viewData)
        {


            var builder = new HtmlContentBuilder();
            var result = FindView(viewName);
            var viewBuffer = new ViewBuffer(_viewBufferScope, result.ViewName, ViewBuffer.PartialViewPageSize);
            using (var writer = new ViewBufferTextWriter(viewBuffer, Encoding.UTF8))
            {
                await RenderPartialViewAsync(writer, model, viewData, result.View);
            }

            return builder.SetHtmlContent(viewBuffer);
                        
        }

        // -----------

        ViewEngineResult FindView(string partialName)
        {

            var viewEngineResult = _viewEngine.GetView(ViewContext.ExecutingFilePath, partialName, isMainPage: false);
            var getViewLocations = viewEngineResult.SearchedLocations;
            if (!viewEngineResult.Success)
            {
                viewEngineResult = _viewEngine.FindView(ViewContext, partialName, isMainPage: false);
            }

            if (!viewEngineResult.Success)
            {
                var searchedLocations = Enumerable.Concat(getViewLocations, viewEngineResult.SearchedLocations);
                return ViewEngineResult.NotFound(partialName, searchedLocations);
            }

            return viewEngineResult;
        }

        async Task RenderPartialViewAsync(
            TextWriter writer,
            object model,
            ViewDataDictionary viewData,
            Microsoft.AspNetCore.Mvc.ViewEngines.IView view)
        {
            // Determine which ViewData we should use to construct a new ViewData
            var baseViewData = viewData ?? ViewContext.ViewData;
            var newViewData = new ViewDataDictionary<object>(baseViewData, model);
            var partialViewContext = new ViewContext(ViewContext, view, newViewData, writer);

            using (view as IDisposable)
            {
                await view.RenderAsync(partialViewContext);
            }
        }

    }


}
