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
            // Apply adaptor builder & return compiled results
            var builder = new ViewAdaptorBuilder();
            configure(builder);
            return Task.FromResult(builder.ViewAdaptorResult.For(name));
        }

    }

}
