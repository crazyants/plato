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

        public Task<IViewAdaptorResult> Adapt(
            string name,
            Action<IViewAdaptorBuilder> configure)
        {

            // Apply adaptor builder
            var builder = new ViewAdaptorBuilder();
            configure(builder);
            
            // Return adapted result
            return Task.FromResult((IViewAdaptorResult)new ViewAdaptorResult()
            {
                ViewName = name,
                AdaptorBuilder = builder
            });

        }

    }

}
