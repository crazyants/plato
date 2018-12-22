using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Drawing.Abstractions;
using Plato.Internal.Drawing.Abstractions.Letters;
using Plato.Internal.Drawing.Letters;

namespace Plato.Internal.Drawing.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPlatoDrawing(
            this IServiceCollection services)
        {

            services.AddScoped<IDisposableBitmap, DisposableBitmap>();
            services.AddScoped<IInMemoryLetterRenderer, InMemoryLetterRenderer>();

            return services;

        }


    }
}
