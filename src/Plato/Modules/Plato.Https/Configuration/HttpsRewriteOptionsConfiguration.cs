using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Options;

namespace Plato.Https.Configuration
{
    class HttpsRewriteOptionsConfiguration : IConfigureOptions<RewriteOptions>
    {

        public void Configure(RewriteOptions options)
        {

            var redirectToSsl = true;
            var requireHttpsPermanent = false;
            var sslPort = 443;

            if (redirectToSsl)
            {
                options.AddRedirectToHttps(
                    requireHttpsPermanent
                        ? StatusCodes.Status301MovedPermanently
                        : StatusCodes.Status302Found,
                    sslPort);
            }


        }


    }
}
