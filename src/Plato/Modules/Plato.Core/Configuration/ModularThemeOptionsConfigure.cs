using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Core.Configuration
{
    //public class ModularThemeOptionsConfigure : IConfigureOptions<ThemeOptions>
    //{

    //    private readonly IContextFacade _contextFacade;
    //    private readonly ISiteSettings _siteSettings;
    //    private readonly IServiceScopeFactory _serviceScopeFactory;

    //    public ModularThemeOptionsConfigure(
    //        IServiceScopeFactory serviceScopeFactory,
    //        ISiteSettings siteSettings,
    //        IContextFacade contextFacade)
    //    {
    //        _siteSettings = siteSettings;
    //        _serviceScopeFactory = serviceScopeFactory;
    //        _contextFacade = contextFacade;
    //    }

    //    public void Configure(ThemeOptions options)
    //    {

    //        using (var scope = _serviceScopeFactory.CreateScope())
    //        {
    //            var configuration = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();
    //            var modulesSection = configuration.GetSection("Plato");
    //            if (modulesSection != null)
    //            {
    //                var children = modulesSection.GetChildren();
    //                foreach (var child in children)
    //                {
    //                    if (child.Key.Contains("VirtualPathToThemesFolder"))
    //                        options.VirtualPathToThemesFolder = child.Value;
    //                }
    //            }
                

    //        }

    //    }

    //}

}
