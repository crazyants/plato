using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Views
{

    public interface IViewFactory
    {
        Task<ViewDescriptor> CreateAsync(IView view);

        Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext);

    }

    public class ViewFactory : IViewFactory
    {

        private readonly IViewTableManager _viewTableManager;
        private readonly IViewInvoker _viewInvoker;

        public ViewFactory(
            IViewTableManager viewTableManager,
            IViewInvoker viewInvoker)
        {
            _viewTableManager = viewTableManager;
            _viewInvoker = viewInvoker;
        }

        public async Task<ViewDescriptor> CreateAsync(IView view)
        {
            return await _viewTableManager.TryAdd(view);
        }

        public async Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext)
        {

            // Contextulize generic view invoker

            _viewInvoker.Contextualize(displayContext);

            // Apply view & model alterations

            if (displayContext.ViewAdaptorResults != null)
            {
                foreach (var viewAdaptorResult in displayContext.ViewAdaptorResults)
                {

                    var updatedView = displayContext.ViewDescriptor.View;

                        // Apply view alterations

                    var viewAlterations = viewAdaptorResult.ViewAlterations;
                        if (viewAlterations.Count > 0)
                        {
                            foreach (var alteration in viewAlterations)
                            {
                                updatedView.ViewName = alteration;
                            }
                        }

                        // Apply model alterations

                        var modelAlterations = viewAdaptorResult.ModelAlterations;
                        if (modelAlterations.Count > 0)
                        {
                            foreach (var alteration in modelAlterations)
                            {
                                updatedView.Model = alteration(updatedView.Model);
                            }
                        }

                        displayContext.ViewDescriptor.View = updatedView;
                    
                }

            }

            // Invoke generic view

            var htmlContent = await _viewInvoker.InvokeAsync(
                displayContext.ViewDescriptor.View.ViewName,
                displayContext.ViewDescriptor.View.Model);

            // Apply adaptor output alterations

            if (displayContext.ViewAdaptorResults != null)
            {
                foreach (var viewAdaptorResult in displayContext.ViewAdaptorResults)
                {
                    var alterations = viewAdaptorResult.OutputAlterations;
                    if (alterations.Count > 0)
                    {
                        foreach (var alteration in alterations)
                        {
                            htmlContent = alteration(htmlContent);
                        }
                    }

                }
            }

            return htmlContent;

        }


    }

}
