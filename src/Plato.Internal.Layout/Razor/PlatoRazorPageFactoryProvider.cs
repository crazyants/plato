using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Plato.Internal.Abstractions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Plato.Internal.Layout.Razor
{

    /// <summary>
    /// An implementation of IRazorPageFactoryProvider that adds simple caching for compiled results.
    /// https://github.com/aspnet/Mvc/blob/f80490f99d4e38e1630e8cce739daf1b3a2b96a8/src/Microsoft.AspNetCore.Mvc.Razor/Compilation/DefaultRazorPageFactoryProvider.cs
    /// </summary>
    public class PlatoRazorPageFactoryProvider : IRazorPageFactoryProvider
    {

        private readonly IViewCompilerProvider _viewCompilerProvider;
        private readonly ISingletonCache<CompiledViewDescriptor> _complierCache;

        public PlatoRazorPageFactoryProvider(
            IViewCompilerProvider viewCompilerProvider,
            ISingletonCache<CompiledViewDescriptor> complierCache)
        {
            _viewCompilerProvider = viewCompilerProvider;
            _complierCache = complierCache;
        }

        private IViewCompiler Compiler => _viewCompilerProvider.GetCompiler();

        /// <inheritdoc />
        public RazorPageFactoryResult CreateFactory(string relativePath)
        {

            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            if (relativePath.StartsWith("~/", StringComparison.Ordinal))
            {
                // For tilde slash paths, drop the leading ~ to make it work with the underlying IFileProvider.
                relativePath = relativePath.Substring(1);
            }

            // Cache our compilation result
            CompiledViewDescriptor viewDescriptor;
            if (!_complierCache.ContainsKey(relativePath))
            {
                // https://github.com/aspnet/Mvc/blob/f80490f99d4e38e1630e8cce739daf1b3a2b96a8/src/Microsoft.AspNetCore.Mvc.Razor/Compilation/RazorViewCompiler.cs
                var compileTask = Compiler.CompileAsync(relativePath);
                viewDescriptor = compileTask.GetAwaiter().GetResult();
                _complierCache[relativePath] = viewDescriptor;
            }
            else
            {
                viewDescriptor = _complierCache[relativePath];
            }
                     
            var viewType = viewDescriptor.Type;
            if (viewType != null)
            {
                var newExpression = Expression.New(viewType);
                var pathProperty = viewType.GetTypeInfo().GetProperty(nameof(IRazorPage.Path));

                // Generate: page.Path = relativePath;
                // Use the normalized path specified from the result.
                var propertyBindExpression = Expression.Bind(pathProperty, Expression.Constant(viewDescriptor.RelativePath));
                var objectInitializeExpression = Expression.MemberInit(newExpression, propertyBindExpression);
                return new RazorPageFactoryResult(viewDescriptor, Expression
                    .Lambda<Func<IRazorPage>>(objectInitializeExpression)
                    .Compile());
            }
            else
            {
                return new RazorPageFactoryResult(viewDescriptor, razorPageFactory: null);
            }
        }
    }
}
