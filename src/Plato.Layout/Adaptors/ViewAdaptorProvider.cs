using System;
using System.Threading.Tasks;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorProvider
    {
        Task<IViewAdaptorResult> ConfigureAsync();
    }

    public abstract class BaseAdaptor : IViewAdaptorProvider
    {

        public abstract Task<IViewAdaptorResult> ConfigureAsync();

        public async Task<IViewAdaptorResult> Adapt(
            string name,
            Action<IViewAdaptorBuilder> configure
        )
        {

            var builder = new ViewAdaptorBuilder(); ;
            configure(builder);

            return new ViewAdaptorResult()
            {
                AdaptorBuilder = builder
            };

        }

    }

}
