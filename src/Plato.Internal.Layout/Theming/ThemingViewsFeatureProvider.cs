using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Layout.ViewFeatures;

namespace Plato.Internal.Layout.Theming
{
    public class ThemingViewsFeatureProvider : IApplicationFeatureProvider<ViewsFeature>
    {


        private readonly IEnumerable<IModularViewsFeatureProvider<ViewsFeature>> _providers;

        public ThemingViewsFeatureProvider(IEnumerable<IModularViewsFeatureProvider<ViewsFeature>> providers)
        {
            _providers = providers;
        }


        /// <summary>
        /// Add theming conventions. Configures theme layout based on controller type.
        /// If a controller is prefixed with Admin the _AdminLayout.cshtml layout file will be used
        /// See ThemeViewStart class for implementation.
        /// </summary>
        /// <param name="parts"></param>
        /// <param name="feature"></param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
        {
            
            if (_providers != null)
            {
                var partsList = parts.ToList();
                foreach (var provider in _providers)
                {
                    provider.PopulateFeature(partsList, feature);
                }
            }

            feature.ViewDescriptors.Add(new CompiledViewDescriptor()
            {
                ExpirationTokens = Array.Empty<IChangeToken>(),
                RelativePath = ViewPath.NormalizePath("/_ViewStart" + RazorViewEngine.ViewExtension),
                ViewAttribute = new RazorViewAttribute("/_ViewStart" + RazorViewEngine.ViewExtension, typeof(ThemeViewStart)),
                IsPrecompiled = true
            });

        }
    }
}
