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


    public class ViewCacheKey 
    {

        private int? _hashcode;

        private const string CacheKeyTokenSeparator = "||";

        // VaryBy 
        private readonly string _varyByViewName;        
        private readonly string _varyByUserName;
        private readonly string _varyByExecutingFilePath;
        private readonly string _varyByRouteValues;

        private readonly object _varyByModel;

        public CacheToken Token { get; }
                     
        public ViewCacheKey(string viewName, object model, ViewContext viewContext)
        {
            var httpContext = viewContext.HttpContext;
        
            _varyByViewName = viewName;
            _varyByModel = model;
            _varyByUserName = httpContext.User?.Identity?.Name ?? string.Empty;            
            _varyByExecutingFilePath = viewContext.ExecutingFilePath;

            var sb = new StringBuilder();
            foreach (var routeValue in viewContext.RouteData.Values)
            {
                sb
                    .Append(routeValue.Key)
                    .Append(CacheKeyTokenSeparator)
                    .Append(routeValue.Value);
                _varyByRouteValues = sb.ToString();
            }

            Token = new CacheToken(model.GetType(),
                _varyByViewName,
                _varyByUserName,
                _varyByExecutingFilePath,
                _varyByRouteValues);
            
        }

        //string _generatedKey;

        //public string GenerateKey()
        //{
        //    // Caching as the key is immutable and it can be called multiple times during a request.
        //    if (_generatedKey != null)
        //    {
        //        return _generatedKey;
        //    }
                       
        //    var sb = new StringBuilder();
        //    sb
        //        .Append(_varyByExecutingFilePath)
        //        .Append(CacheKeyTokenSeparator)                
        //        .Append(_varyByViewName)
        //        .Append(CacheKeyTokenSeparator)
        //        .Append(_varyByUserName)
        //           .Append(CacheKeyTokenSeparator)
        //        .Append(_varyByRouteValues);

        //    _generatedKey = sb.ToString();

        //    return _generatedKey;

        //}

        //public string GenerateHashedKey()
        //{
        //    var key = GenerateKey();

        //    // The key is typically too long to be useful, so we use a cryptographic hash
        //    // as the actual key (better randomization and key distribution, so small vary
        //    // values will generate dramatically different keys).
        //    using (var sha256 = CryptographyAlgorithms.CreateSHA256())
        //    {
        //        var contentBytes = Encoding.UTF8.GetBytes(key);
        //        var hashedBytes = sha256.ComputeHash(contentBytes);
        //        return Convert.ToBase64String(hashedBytes);
        //    }
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj is ViewCacheKey other)
        //    {
        //        return Equals(other);
        //    }

        //    return false;
        //}

        //public bool Equals(ViewCacheKey other)
        //{
        //    return string.Equals(other._varyByViewName, _varyByViewName, StringComparison.Ordinal)
        //        && string.Equals(other._varyByUserName, _varyByUserName, StringComparison.Ordinal)
        //        && string.Equals(other._varyByExecutingFilePath, _varyByExecutingFilePath, StringComparison.Ordinal)
        //        && string.Equals(other._varyByRouteValues, _varyByRouteValues, StringComparison.Ordinal);
        //}

        //public override int GetHashCode()
        //{
        //    // The hashcode is intentionally not using the computed
        //    // stringified key in order to prevent string allocations
        //    // in the common case where it's not explicitly required.

        //    // Caching as the key is immutable and it can be called
        //    // multiple times during a request.
        //    if (_hashcode.HasValue)
        //    {
        //        return _hashcode.Value;
        //    }

        //    var hashCodeCombiner = new HashCodeCombiner();

        //    hashCodeCombiner.Add(_varyByViewName, StringComparer.Ordinal);
        //    hashCodeCombiner.Add(_varyByExecutingFilePath, StringComparer.Ordinal);
        //    hashCodeCombiner.Add(_varyByUserName, StringComparer.Ordinal);
        //    hashCodeCombiner.Add(_varyByRouteValues, StringComparer.Ordinal);

        //    hashCodeCombiner.Add(_varyByModel);

        //    //hashCodeCombiner.Add(_expiresOn);
        //    //hashCodeCombiner.Add(_expiresSliding);
        //    //hashCodeCombiner.Add(_varyBy, StringComparer.Ordinal);
        //    //hashCodeCombiner.Add(_username, StringComparer.Ordinal);
        //    //hashCodeCombiner.Add(_requestCulture);
        //    //hashCodeCombiner.Add(_requestUICulture);

        //    //CombineCollectionHashCode(hashCodeCombiner, VaryByCookieName, _cookies);
        //    //CombineCollectionHashCode(hashCodeCombiner, VaryByHeaderName, _headers);
        //    //CombineCollectionHashCode(hashCodeCombiner, VaryByQueryName, _queries);
        //    //CombineCollectionHashCode(hashCodeCombiner, VaryByRouteName, _routeValues);

        //    _hashcode = hashCodeCombiner.CombinedHash;

        //    return _hashcode.Value;

        //}

        //public override string ToString()
        //{
        //    return Key;
        //}

    }

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
            
            //var viewToken = new ViewCacheKey(viewName, model, ViewContext);
            //var key = viewToken.Token.ToString();

            //IHtmlContent content;
            //if (_memoryCache.TryGetValue(key, out Task<IHtmlContent> cachedResult))
            //{
            //    // There is either some value already cached (as a Task) or a worker processing the output.
            //    content = await cachedResult;
            //}
            //else
            //{
            //    content = await CreateCacheEntry(key, viewName, model, viewData);
            //}

            //return content;
            
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

        //private async Task<IHtmlContent> CreateCacheEntry(
        //    string key,
        //    string viewName,
        //    object model,
        //    ViewDataDictionary viewData)
        //{

        //    var tokenSource = new CancellationTokenSource();

        //    var options = GetMemoryCacheEntryOptions();
        //    options.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));

        //    var tcs = new TaskCompletionSource<IHtmlContent>(creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

        //    // The returned value is ignored, we only do this so that
        //    // the compiler doesn't complain about the returned task
        //    // not being awaited
        //    _ = _memoryCache.Set(key, tcs.Task, options);

        //    IHtmlContent content;
        //    try
        //    {
        //        // The entry is set instead of assigning a value to the
        //        // task so that the expiration options are not impacted
        //        // by the time it took to compute it.

        //        // Use the CreateEntry to ensure a cache scope is created that will copy expiration tokens from
        //        // cache entries created from the GetChildContentAsync call to the current entry.
        //        var entry = _memoryCache.CreateEntry(key);

        //        // The result is processed inside an entry
        //        // such that the tokens are inherited.

        //        var result = ProcessContentAsync(viewName, model, viewData);
        //        content = await result;

        //        entry.SetOptions(options);

        //        entry.Value = result;

        //        // An entry gets committed to the cache when disposed gets called. We only want to do this when
        //        // the content has been correctly generated (didn't throw an exception). For that reason the entry
        //        // can't be put inside a using block.
        //        entry.Dispose();

        //        // Set the result on the TCS once we've committed the entry to the cache since commiting to the cache
        //        // may throw.
        //        tcs.SetResult(content);
        //        return content;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Remove the worker task from the cache in case it can't complete.
        //        tokenSource.Cancel();

        //        // Fail the TCS so other awaiters see the exception.
        //        tcs.TrySetException(ex);
        //        throw;
        //    }
        //    finally
        //    {
        //        // The tokenSource needs to be disposed as the MemoryCache
        //        // will register a callback on the Token.
        //        tokenSource.Dispose();
        //    }
        //}

        //internal MemoryCacheEntryOptions GetMemoryCacheEntryOptions()
        //{         
        //    var options = new MemoryCacheEntryOptions();
        //    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));         
        //    return options;
        //}

        //private async Task<IHtmlContent> ProcessContentAsync(
        //    string viewName,
        //    object model,
        //    ViewDataDictionary viewData)
        //{

        //    var builder = new HtmlContentBuilder();
        //    var result = FindView(viewName);
        //    var viewBuffer = new ViewBuffer(_viewBufferScope, result.ViewName, ViewBuffer.PartialViewPageSize);
        //    using (var writer = new ViewBufferTextWriter(viewBuffer, Encoding.UTF8))
        //    {
        //        await RenderPartialViewAsync(writer, model, viewData, result.View);
        //    }

        //    using (var writer = new CharBufferTextWriter())
        //    {
        //        viewBuffer.WriteTo(writer, HtmlEncoder.Default);
        //        return new CharBufferHtmlContent(writer.Buffer);
        //    }

        //}

        //private class CharBufferTextWriter : TextWriter
        //{
        //    public CharBufferTextWriter()
        //    {
        //        Buffer = new PagedCharBuffer(CharArrayBufferSource.Instance);
        //    }

        //    public override Encoding Encoding => Null.Encoding;

        //    public PagedCharBuffer Buffer { get; }

        //    public override void Write(char value)
        //    {
        //        Buffer.Append(value);
        //    }

        //    public override void Write(char[] buffer, int index, int count)
        //    {
        //        Buffer.Append(buffer, index, count);
        //    }

        //    public override void Write(string value)
        //    {
        //        Buffer.Append(value);
        //    }
        //}

        //private class CharBufferHtmlContent : IHtmlContent
        //{
        //    private readonly PagedCharBuffer _buffer;

        //    public CharBufferHtmlContent(PagedCharBuffer buffer)
        //    {
        //        _buffer = buffer;
        //    }

        //    public PagedCharBuffer Buffer => _buffer;

        //    public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        //    {
        //        var length = Buffer.Length;
        //        if (length == 0)
        //        {
        //            return;
        //        }

        //        for (var i = 0; i < Buffer.Pages.Count; i++)
        //        {
        //            var page = Buffer.Pages[i];
        //            var pageLength = Math.Min(length, page.Length);
        //            writer.Write(page, index: 0, count: pageLength);
        //            length -= pageLength;
        //        }

        //    }
        //}

    }


}
