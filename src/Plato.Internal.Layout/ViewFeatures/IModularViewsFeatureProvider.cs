using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Primitives;
using Plato.Internal.Hosting.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Plato.Internal.Layout.ViewFeatures
{
    public interface IModularViewsFeatureProvider<TFeature> : IApplicationFeatureProvider<TFeature>
    {
    }
    

}
