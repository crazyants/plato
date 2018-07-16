using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;
using Plato.Https.Models;
using Plato.Https.Stores;

namespace Plato.Https.Configuration
{
    class HttpsRewriteOptionsConfiguration : IConfigureOptions<RewriteOptions>
    {

        private readonly IHttpsSettingsStore<HttpsSettings> _httpsSettingsStore;

        public HttpsRewriteOptionsConfiguration(IHttpsSettingsStore<HttpsSettings> httpsSettingsStore)
        {
            _httpsSettingsStore = httpsSettingsStore;
        }

        public void Configure(RewriteOptions options)
        {

            // Defaults
            var enforeSsl = false;
            var usePermanentRedirect = false;
            var sslPort = 443;

            // Get settings
            var settings = _httpsSettingsStore.GetAsync().GetAwaiter().GetResult();

            if (settings != null)
            {
                enforeSsl = settings.EnforceSsl;
                usePermanentRedirect = settings.UsePermanentRedirect;
                sslPort = settings.SslPort;
            }
       
            if (enforeSsl)
            {
                options.AddRedirectToHttps(
                    usePermanentRedirect
                        ? StatusCodes.Status301MovedPermanently
                        : StatusCodes.Status302Found,
                    sslPort);
            }


        }


    }
}
