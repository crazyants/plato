using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Plato.Layout.Views
{

    public interface IGenericViewFactory
    {
        Task<GenericViewDescriptor> CreateAsync(IGenericView view);

        Task<IHtmlContent> InvokeAsync(GenericViewDisplayContext displayContext);

    }

    public class GenericViewFactory : IGenericViewFactory
    {

        private readonly IGenericViewTableManager _genericViewTableManager;
        private readonly IGenericViewInvoker _genericViewInvoker;

        public GenericViewFactory(
            IGenericViewTableManager genericViewTableManager,
            IGenericViewInvoker genericViewInvoker)
        {
            _genericViewTableManager = genericViewTableManager;
            _genericViewInvoker = genericViewInvoker;
        }

        public async Task<GenericViewDescriptor> CreateAsync(IGenericView view)
        {
            return await _genericViewTableManager.TryAdd(view);
        }

        public async Task<IHtmlContent> InvokeAsync(GenericViewDisplayContext displayContext)
        {

            // Contextulize generic view invoker

            _genericViewInvoker.Contextualize(displayContext);

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

            var htmlContent = await _genericViewInvoker.InvokeAsync(
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
