using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Layout.ModelBinding;

namespace Plato.Internal.Layout.ViewProviders
{

    public class ViewProviderManager<TModel> : IViewProviderManager<TModel> where TModel : class
    {

        private readonly IEnumerable<IViewProvider<TModel>> _providers;
        private readonly ILogger<ViewProviderManager<TModel>> _logger;

        public ViewProviderManager(
            IEnumerable<IViewProvider<TModel>> providers,
            ILogger<ViewProviderManager<TModel>> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public async Task<IViewProviderResult> ProvideDisplayAsync(TModel model, IUpdateModel context)
        {

            var results = new ConcurrentBag<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                try
                {
                    results.Add(await provider.BuildDisplayAsync(model, new ViewProviderContext(context)));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred within the view providers BuildDisplayAsync method. Please review the BuildDisplayAsync method within your view provider and try again. {e.Message}");
                    throw;
                }
            }

            return new LayoutViewModel(results.ToArray()).BuildLayout();

        }

        public async Task<IViewProviderResult> ProvideIndexAsync(TModel model, IUpdateModel updater)
        {
            var results = new ConcurrentBag<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                try
                {
                    results.Add(await provider.BuildIndexAsync(model, new ViewProviderContext(updater)));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred within the view providers BuildLayoutAsync method. Please review the BuildLayoutAsync method within your view provider and try again. {e.Message}");
                    throw;
                }
            }

            return new LayoutViewModel(results.ToArray()).BuildLayout();

        }

        public async Task<IViewProviderResult> ProvideEditAsync(TModel model, IUpdateModel updater)
        {

            var results = new ConcurrentBag<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                try
                {
                    results.Add(await provider.BuildEditAsync(model, new ViewProviderContext(updater)));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred within the view providers BuildEditAsync method. Please review the BuildEditAsync method within your view provider and try again. {e.Message}");
                    throw;
                }
            }

            return new LayoutViewModel(results.ToArray()).BuildLayout();

        }

        public async Task<IViewProviderResult> ProvideUpdateAsync(TModel model, IUpdateModel updater)
        {

            var results = new ConcurrentBag<IViewProviderResult>();
            foreach (var provider in _providers)
            {
                try
                {
                    results.Add(await provider.BuildUpdateAsync(model, new ViewProviderContext(updater)));
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred within the view providers BuildUpdateAsync method. Please review the UpdateAsync method within your view provider and try again. {e.Message}");
                    throw;
                }
            }

            return new LayoutViewModel(results.ToArray()).BuildLayout();

        }

        public async Task<bool> IsModelStateValidAsync(TModel model, IUpdateModel updater)
        {
            var valid = true;
            foreach (var provider in _providers)
            {
                try
                {
                    if (!await provider.ValidateModelAsync(model, updater))
                    {
                        valid = false;
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred within the view providers IsModelStateValid method. Please review the ValidateModelAsync method within your view provider and try again. {e.Message}");
                    throw;
                }
            }

            return valid;

        }

        public async Task<TModel> ComposeModelAsync(TModel model, IUpdateModel context)
        {
            foreach (var provider in _providers)
            {
                try
                {
                    await provider.ComposeModelAsync(model, context);

                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred within the view providers GetComposedModel method. Please review the ComposedModel method within your view provider and try again. {e.Message}");
                    throw;
                }
            }

            return model;

        }

    }

}