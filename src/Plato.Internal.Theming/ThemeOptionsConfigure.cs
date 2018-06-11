using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Plato.Internal.Theming
{
    public class ThemeOptionsConfigure : IConfigureOptions<ThemeOptions>
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ThemeOptionsConfigure(
            IServiceScopeFactory serivceScopeFactory)
        {
            _serviceScopeFactory = serivceScopeFactory;
        }

        public void Configure(ThemeOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();

                var modulesSection = configuration.GetSection("Plato");
                if (modulesSection != null)
                {
                    var children = modulesSection.GetChildren();
                    foreach (var child in children)
                    {
                        if (child.Key.Contains("VirtualPathToThemesFolder"))
                            options.VirtualPathToThemesFolder = child.Value;
                    }

                }

            }

        }

    }
}
