using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Plato.Internal.Text.Abstractions;
using Plato.Internal.Text.Alias;
using Plato.Internal.Text.UriExtractors;

namespace Plato.Internal.Text.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoText(
            this IServiceCollection services)
        {


            services.TryAddSingleton<IAliasCreator, AliasCreator>();

            services.TryAddSingleton<IImageUriExtractor, ImageUriExtractor>();
            services.TryAddSingleton<IAnchorUriExtractor, AnchorUriExtractor>();
            services.TryAddSingleton<IKeyGenerator, KeyGenerator>();
            services.TryAddSingleton<IHtmlSanitizer, HtmlSanitizer>();

            services.TryAddSingleton<ITextParser, TextParser>();
        
            return services;

        }


    }
}
