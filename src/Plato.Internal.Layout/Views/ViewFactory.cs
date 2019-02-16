using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.Views
{
    
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

        public ViewDescriptor Create(IView view)
        {
            return _viewTableManager.TryAdd(view);
        }

        public async Task<IHtmlContent> InvokeAsync(ViewDisplayContext displayContext)
        {

            // Contextulize view invoker
            _viewInvoker.Contextualize(displayContext);

            // Apply view & model alterations
            if (displayContext.ViewAdaptorResults != null)
            {
                foreach (var viewAdapterResult in displayContext.ViewAdaptorResults)
                {

                    var updatedView = displayContext.ViewDescriptor.View;

                    // Apply view alterations
                    var viewAlterations = viewAdapterResult.ViewAlterations;
                    if (viewAlterations.Count > 0)
                    {
                        foreach (var alteration in viewAlterations)
                        {
                            updatedView.ViewName = alteration;
                        }
                    }

                    // Apply model alterations
                    var modelAlterations = viewAdapterResult.ModelAlterations;
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
            var htmlContent = await _viewInvoker.InvokeAsync(displayContext.ViewDescriptor.View);

            // Apply adapter output alterations
            if (displayContext.ViewAdaptorResults != null)
            {
                foreach (var viewAdapterResult in displayContext.ViewAdaptorResults)
                {
                    var alterations = viewAdapterResult.OutputAlterations;
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
