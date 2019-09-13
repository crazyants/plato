using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Plato.Internal.Layout.Razor
{
     
    /// <summary>
    /// Optimized implementation of RazorViewEngine for Plato.
    /// </summary>
    public class PlatoViewEngine : IRazorViewEngine
    {

        public static readonly string ViewExtension = ".cshtml";

        private const string AreaKey = "area";
        private const string ControllerKey = "controller";
        private const string PageKey = "page";

        private static readonly TimeSpan _cacheExpirationDuration = TimeSpan.FromHours(48);

        protected IMemoryCache ViewLookupCache { get; }

        public static string GetNormalizedRouteValue(ActionContext context, string key)
                 => NormalizedRouteValue.GetNormalizedRouteValue(context, key);
        
        private readonly DiagnosticListener _diagnosticListener;
        private readonly IRazorPageFactoryProvider _pageFactory;   
        private readonly IRazorPageActivator _pageActivator;
        private readonly RazorViewEngineOptions _options;
        private readonly HtmlEncoder _htmlEncoder;
        
        public PlatoViewEngine(
            IRazorPageFactoryProvider pageFactory,
            IOptions<RazorViewEngineOptions> optionsAccessor,
            DiagnosticListener diagnosticListener,            
            IRazorPageActivator pageActivator,
            HtmlEncoder htmlEncoder)
        {
            _diagnosticListener = diagnosticListener;
            _options = optionsAccessor.Value;            
            _pageActivator = pageActivator;                      
            _pageFactory = pageFactory;
            _htmlEncoder = htmlEncoder;       
            ViewLookupCache = new MemoryCache(new MemoryCacheOptions());
        }

        public RazorPageResult FindPage(ActionContext context, string pageName)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrEmpty(pageName))
            {
                throw new ArgumentNullException(nameof(pageName));
            }

            if (IsApplicationRelativePath(pageName) || IsRelativePath(pageName))
            {
                // A path; not a name this method can handle.
                return new RazorPageResult(pageName, Enumerable.Empty<string>());
            }

            var cacheResult = LocatePageFromViewLocations(context, pageName, isMainPage: false);
            if (cacheResult.Success)
            {
                var razorPage = cacheResult.ViewEntry.PageFactory();
                return new RazorPageResult(pageName, razorPage);
            }
            else
            {
                return new RazorPageResult(pageName, cacheResult.SearchedLocations);
            }
        }
            
        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            var cacheResult = LocatePageFromViewLocations(context, viewName, isMainPage);
            return CreateViewEngineResult(cacheResult, viewName);

        }
             
        private ViewLocationCacheResult LocatePageFromViewLocations(
             ActionContext actionContext,
             string pageName,
             bool isMainPage)
        {

            var controllerName = GetNormalizedRouteValue(actionContext, ControllerKey);
            var areaName = GetNormalizedRouteValue(actionContext, AreaKey);
            string razorPageName = null;
            if (actionContext.ActionDescriptor.RouteValues.ContainsKey(PageKey))
            {
                // Only calculate the Razor Page name if "page" is registered in RouteValues.
                razorPageName = GetNormalizedRouteValue(actionContext, PageKey);
            }

            var expanderContext = new ViewLocationExpanderContext(
                actionContext,
                pageName,
                controllerName,
                areaName,
                razorPageName,
                isMainPage);
            Dictionary<string, string> expanderValues = null;

            if (_options.ViewLocationExpanders.Count > 0)
            {
                expanderValues = new Dictionary<string, string>(StringComparer.Ordinal);
                expanderContext.Values = expanderValues;

                ////// Perf: Avoid allocations
                ////for (var i = 0; i < _options.ViewLocationExpanders.Count; i++)
                ////{
                ////    _options.ViewLocationExpanders[i].PopulateValues(expanderContext);
                ////}
            }

            var cacheKey = new ViewLocationCacheKey(
                expanderContext.ViewName,
                expanderContext.ControllerName,
                expanderContext.AreaName,
                expanderContext.PageName,
                expanderContext.IsMainPage,
                expanderValues);

            if (!ViewLookupCache.TryGetValue(cacheKey, out ViewLocationCacheResult cacheResult))
            {                
                cacheResult = OnCacheMiss(expanderContext, cacheKey);
            }
         
            return cacheResult;

        }

        private ViewEngineResult CreateViewEngineResult(ViewLocationCacheResult result, string viewName)
        {

            if (!result.Success)
            {
                return ViewEngineResult.NotFound(viewName, result.SearchedLocations);
            }

            var page = result.ViewEntry.PageFactory();

            var viewStarts = new IRazorPage[result.ViewStartEntries.Count];
            for (var i = 0; i < viewStarts.Length; i++)
            {
                var viewStartItem = result.ViewStartEntries[i];
                viewStarts[i] = viewStartItem.PageFactory();
            }

            var view = new RazorView(this, _pageActivator, viewStarts, page, _htmlEncoder, _diagnosticListener);
            return ViewEngineResult.Found(viewName, view);
        }

        private IEnumerable<string> _viewLocations;

        ConcurrentDictionary<string, IEnumerable<string>> _viewLocations1 =
            new ConcurrentDictionary<string, IEnumerable<string>>();

        private ViewLocationCacheResult OnCacheMiss(
          ViewLocationExpanderContext expanderContext,
          ViewLocationCacheKey cacheKey)
        {

            var sb = new StringBuilder();
            sb.Append(expanderContext.ViewName)
                .Append(expanderContext.ControllerName)
                .Append(expanderContext.AreaName);

            var key = sb.ToString();

            if (!_viewLocations1.ContainsKey(key))
            {

                var viewLocations = GetViewLocationFormats(expanderContext);
                for (var i = 0; i < _options.ViewLocationExpanders.Count; i++)
                {
                    viewLocations = _options.ViewLocationExpanders[i]
                        .ExpandViewLocations(expanderContext, viewLocations);
                }

                var resolvedLocations = new List<string>();
                foreach (var location in viewLocations)
                {
                    var path = string.Format(
                          CultureInfo.InvariantCulture,
                          location,
                          expanderContext.ViewName,
                          expanderContext.ControllerName,
                          expanderContext.AreaName);

                    path = ViewEnginePath.ResolvePath(path);
                    resolvedLocations.Add(path);
                }

                _viewLocations1[key] = resolvedLocations;

            }

            ViewLocationCacheResult cacheResult = null;
            var searchedLocations = new List<string>();
            var expirationTokens = new HashSet<IChangeToken>();

            foreach (var path in _viewLocations1[key])
            {
                cacheResult = CreateCacheResult(expirationTokens, path, expanderContext.IsMainPage);
                if (cacheResult != null)
                {
                    break;
                }

                searchedLocations.Add(path);
            }

            // No views were found at the specified location. Create a not found result.
            if (cacheResult == null)
            {
                cacheResult = new ViewLocationCacheResult(searchedLocations);
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions();
            cacheEntryOptions.SetSlidingExpiration(_cacheExpirationDuration);
            //foreach (var expirationToken in expirationTokens)
            //{
            //    cacheEntryOptions.AddExpirationToken(expirationToken);
            //}

            return ViewLookupCache.Set(cacheKey, cacheResult, cacheEntryOptions);
        }

        private ConcurrentDictionary<string, RazorPageFactoryResult> pageFactoryResults =
             new ConcurrentDictionary<string, RazorPageFactoryResult>();

        // Internal for unit testing
        internal ViewLocationCacheResult CreateCacheResult(
            HashSet<IChangeToken> expirationTokens,
            string relativePath,
            bool isMainPage)
        {

            var factoryResult = _pageFactory.CreateFactory(relativePath);
            var viewDescriptor = factoryResult.ViewDescriptor;
            if (viewDescriptor?.ExpirationTokens != null)
            {
                for (var i = 0; i < viewDescriptor.ExpirationTokens.Count; i++)
                {
                    expirationTokens.Add(viewDescriptor.ExpirationTokens[i]);
                }
            }

            if (factoryResult.Success)
            {
                // Only need to lookup _ViewStarts for the main page.
                var viewStartPages = isMainPage ?
                    GetViewStartPages(viewDescriptor.RelativePath, expirationTokens) :
                    Array.Empty<ViewLocationCacheItem>();

                return new ViewLocationCacheResult(
                    new ViewLocationCacheItem(factoryResult.RazorPageFactory, relativePath),
                    viewStartPages);
            }

            return null;
        }

        private IReadOnlyList<ViewLocationCacheItem> GetViewStartPages(
            string path,
            HashSet<IChangeToken> expirationTokens)
        {
            var viewStartPages = new List<ViewLocationCacheItem>();

            foreach (var filePath in RazorFileHierarchy.GetViewStartPaths(path))
            {
                var result = _pageFactory.CreateFactory(filePath);
                var viewDescriptor = result.ViewDescriptor;
                if (viewDescriptor?.ExpirationTokens != null)
                {
                    for (var i = 0; i < viewDescriptor.ExpirationTokens.Count; i++)
                    {
                        expirationTokens.Add(viewDescriptor.ExpirationTokens[i]);
                    }
                }

                if (result.Success)
                {
                    // Populate the viewStartPages list so that _ViewStarts appear in the order the need to be
                    // executed (closest last, furthest first). This is the reverse order in which
                    // ViewHierarchyUtility.GetViewStartLocations returns _ViewStarts.
                    viewStartPages.Insert(0, new ViewLocationCacheItem(result.RazorPageFactory, filePath));
                }
            }

            return viewStartPages;
        }


        internal IEnumerable<string> GetViewLocationFormats(ViewLocationExpanderContext context)
        {
            if (!string.IsNullOrEmpty(context.AreaName) &&
                !string.IsNullOrEmpty(context.ControllerName))
            {
                return _options.AreaViewLocationFormats;
            }
            else if (!string.IsNullOrEmpty(context.ControllerName))
            {
                return _options.ViewLocationFormats;
            }
            else if (!string.IsNullOrEmpty(context.AreaName) &&
                !string.IsNullOrEmpty(context.PageName))
            {
                return _options.AreaPageViewLocationFormats;
            }
            else if (!string.IsNullOrEmpty(context.PageName))
            {
                return _options.PageViewLocationFormats;
            }
            else
            {
                // If we don't match one of these conditions, we'll just treat it like regular controller/action
                // and use those search paths. This is what we did in 1.0.0 without giving much thought to it.
                return _options.ViewLocationFormats;
            }
        }

        public string GetAbsolutePath(string executingFilePath, string pagePath)
        {
            if (string.IsNullOrEmpty(pagePath))
            {
                // Path is not valid; no change required.
                return pagePath;
            }

            if (IsApplicationRelativePath(pagePath))
            {
                // An absolute path already; no change required.
                return pagePath;
            }

            if (!IsRelativePath(pagePath))
            {
                // A page name; no change required.
                return pagePath;
            }

            if (string.IsNullOrEmpty(executingFilePath))
            {
                // Given a relative path i.e. not yet application-relative (starting with "~/" or "/"), interpret
                // path relative to currently-executing view, if any.
                // Not yet executing a view. Start in app root.
                var absolutePath = "/" + pagePath;
                return ViewEnginePath.ResolvePath(absolutePath);
            }

            return ViewEnginePath.CombinePath(executingFilePath, pagePath);
        }

        public RazorPageResult GetPage(string executingFilePath, string pagePath)
        {
            if (string.IsNullOrEmpty(pagePath))
            {
                throw new ArgumentNullException(nameof(pagePath));
            }

            if (!(IsApplicationRelativePath(pagePath) || IsRelativePath(pagePath)))
            {
                // Not a path this method can handle.
                return new RazorPageResult(pagePath, Enumerable.Empty<string>());
            }

            var cacheResult = LocatePageFromPath(executingFilePath, pagePath, isMainPage: false);
            if (cacheResult.Success)
            {
                var razorPage = cacheResult.ViewEntry.PageFactory();
                return new RazorPageResult(pagePath, razorPage);
            }
            else
            {
                return new RazorPageResult(pagePath, cacheResult.SearchedLocations);
            }
        }
        
        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {

            if (string.IsNullOrEmpty(viewPath))
            {
                throw new ArgumentNullException(nameof(viewPath));
            }

            if (!(IsApplicationRelativePath(viewPath) || IsRelativePath(viewPath)))
            {
                // Not a path this method can handle.
                return ViewEngineResult.NotFound(viewPath, Enumerable.Empty<string>());
            }

            var cacheResult = LocatePageFromPath(executingFilePath, viewPath, isMainPage);
            return CreateViewEngineResult(cacheResult, viewPath);
        }

        private ViewLocationCacheResult LocatePageFromPath(string executingFilePath, string pagePath, bool isMainPage)
        {
            var applicationRelativePath = GetAbsolutePath(executingFilePath, pagePath);
            var cacheKey = new ViewLocationCacheKey(applicationRelativePath, isMainPage);
            if (!ViewLookupCache.TryGetValue(cacheKey, out ViewLocationCacheResult cacheResult))
            {
                var expirationTokens = new HashSet<IChangeToken>();
                cacheResult = CreateCacheResult(expirationTokens, applicationRelativePath, isMainPage);

                var cacheEntryOptions = new MemoryCacheEntryOptions();
                cacheEntryOptions.SetSlidingExpiration(_cacheExpirationDuration);
                foreach (var expirationToken in expirationTokens)
                {
                    cacheEntryOptions.AddExpirationToken(expirationToken);
                }

                // No views were found at the specified location. Create a not found result.
                if (cacheResult == null)
                {
                    cacheResult = new ViewLocationCacheResult(new[] { applicationRelativePath });
                }

                cacheResult = ViewLookupCache.Set(
                    cacheKey,
                    cacheResult,
                    cacheEntryOptions);
            }

            return cacheResult;
        }

        static bool IsApplicationRelativePath(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));
            return name[0] == '~' || name[0] == '/';
        }

        static bool IsRelativePath(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name));

            // Though ./ViewName looks like a relative path, framework searches for that view using view locations.
            return name.EndsWith(ViewExtension, StringComparison.OrdinalIgnoreCase);
        }
        
        internal static class RazorFileHierarchy
        {
            private const string ViewStartFileName = "_ViewStart.cshtml";

            public static IEnumerable<string> GetViewStartPaths(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException(nameof(path));
                }

                if (path[0] != '/')
                {
                    throw new ArgumentNullException(nameof(path));
                }

                if (path.Length == 1)
                {
                    yield break;
                }

                var builder = new StringBuilder(path);
                var maxIterations = 255;
                var index = path.Length;
                while (maxIterations-- > 0 && index > 1 && (index = path.LastIndexOf('/', index - 1)) != -1)
                {
                    builder.Length = index + 1;
                    builder.Append(ViewStartFileName);

                    var itemPath = builder.ToString();
                    yield return itemPath;
                }
            }
        }

    }

}
