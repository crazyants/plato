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
        private readonly ConcurrentDictionary<string, ViewEngineResult> _cache
            = new ConcurrentDictionary<string, ViewEngineResult>();

        private readonly ISingletonCache<HtmlContentBuilder> _singletonCache;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IViewBufferScope _viewBufferScope;
        
        public PartialInvoker(
            ISingletonCache<HtmlContentBuilder> singletonCache,
            ICompositeViewEngine viewEngine,
            IViewBufferScope viewBufferScope)
        {
            _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
            _viewBufferScope = viewBufferScope ?? throw new ArgumentNullException(nameof(viewBufferScope));
            _singletonCache = singletonCache ?? throw new ArgumentNullException(nameof(singletonCache)); ;
        }
        
        public ViewContext ViewContext { get; set; }

        public void Contextualize(ViewContext viewContext)
        {
            this.ViewContext = viewContext;
        }

        public async Task<IHtmlContent> InvokeAsync(string viewName, object model, ViewDataDictionary viewData)
        {
            
            var builder = new HtmlContentBuilder();

            // Cached to avoid FindView calls
            var result =  FindView(viewName);
          
            // Build output
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
