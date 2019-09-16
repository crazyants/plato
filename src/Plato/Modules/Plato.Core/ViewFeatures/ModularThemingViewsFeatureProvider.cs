using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Layout.ViewFeatures;

namespace Plato.Core.ViewFeatures
{
    //public class ModularThemingViewsFeatureProvider : IModularViewsFeatureProvider<ViewsFeature>
    //{

    //    /// Add theming conventions. Configures theme layout based on controller type.
    //    /// If a controller is prefixed with Admin the _AdminLayout.cshtml layout file will be used
    //    /// See ModularThemeViewStart for implementation

    //    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ViewsFeature feature)
    //    {
    //        feature.ViewDescriptors.Add(new CompiledViewDescriptor()
    //        {
    //            ExpirationTokens = Array.Empty<IChangeToken>(),
    //            RelativePath = ViewPath.NormalizePath("/_ViewStart" + RazorViewEngine.ViewExtension),
    //            ViewAttribute = new RazorViewAttribute("/_ViewStart" + RazorViewEngine.ViewExtension, typeof(ModularThemeViewStart)),
    //            IsPrecompiled = true
    //        });
    //    }
    //}
}
