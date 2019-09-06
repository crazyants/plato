using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Internal.Abstractions.Settings;

namespace Plato.Internal.Hosting.Web.Configuration
{
    
    public class PlatoOptionsConfiguration : IConfigureOptions<PlatoOptions>
    {

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PlatoOptionsConfiguration(
            IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(PlatoOptions options)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfigurationRoot>();

                var section = configuration.GetSection("Plato");
                if (section != null)
                {

                    var children = section.GetChildren();
                    foreach (var child in children)
                    {

                        // Version
                        if (child.Key.Contains("Version"))
                        {
                            options.Version = child.Value;
                        }

                        // ReleaseType
                        if (child.Key.Contains("ReleaseType"))
                        {
                            options.ReleaseType = child.Value;
                        }

                        // SecretsPath
                        if (child.Key.Contains("SecretsPath"))
                        {
                            options.SecretsPath = child.Value;
                        }
                        
                        // DemoMode
                        if (child.Key.Contains("DemoMode"))
                        {
                            var ok = bool.TryParse(child.Value, out var result);
                            if (ok)
                            {
                                options.DemoMode = result;
                            }
                        }
                         
                    }

                }

            }

        }

    }

}
