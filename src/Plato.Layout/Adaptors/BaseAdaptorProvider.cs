using System;
using System.Threading.Tasks;

namespace Plato.Layout.Adaptors
{
    public abstract class BaseAdaptorProvider : IViewAdaptorProvider
    {

        private string _viewName;

        public string ViewName => _viewName;

        public abstract Task<IViewAdaptorResult> ConfigureAsync();

        public Task<IViewAdaptorResult> Adapt(
            string viewName,
            Action<IViewAdaptorBuilder> configure)
        {

            // Which view is our adaptor modifying
            _viewName = viewName;

            // Apply adaptor builder & return compiled results
            var builder = new ViewAdaptorBuilder(viewName);
            configure(builder);

            // Ensure results are aware of the builder that created them
            var result = builder.ViewAdaptorResult;
            result.Builder = builder;

            // Return results
            return Task.FromResult(result);

        }

    }
    
}
