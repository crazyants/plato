using System;
using System.Threading.Tasks;

namespace Plato.Internal.Layout.ViewAdapters
{
    public abstract class BaseAdapterProvider : IViewAdapterProvider
    {
        
        public string ViewName { get; set; }

        public abstract Task<IViewAdapterResult> ConfigureAsync(string viewName);

        public Task<IViewAdapterResult> Adapt(string viewName,
            Action<IViewAdapterBuilder> configure)
        {

            // Which view is our adapter modifying
            // Not important mainly used for logging purposes
            //ViewName = viewName;

            // Apply adapter builder & return compiled results
            var builder = new ViewAdapterBuilder(viewName);
            configure(builder);

            // Ensure results are aware of the builder that created them
            var result = builder.ViewAdapterResult;
            result.Builder = builder;

            // Return results
            return Task.FromResult(result);

        }

    }
    
}
