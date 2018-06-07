using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorProvider
    {
        
        string ViewName { get; }

        Task<IViewAdaptorResult> ConfigureAsync();
    }

    public abstract class BaseAdaptorProvider : IViewAdaptorProvider
    {

        private string _viewName;

        public string ViewName => _viewName;

        public abstract Task<IViewAdaptorResult> ConfigureAsync();

        public Task<IViewAdaptorResult> Adapt(
            string viewName,
            Action<IViewAdaptorBuilder> configure)
        {

            // Set viewname - not important but helpful for logging purposes
            _viewName = viewName;

            // Apply adaptor builder & return compiled results
            var builder = new ViewAdaptorBuilder();
            configure(builder);
            return Task.FromResult(builder.ViewAdaptorResult.For(viewName));

        }

    }

}
