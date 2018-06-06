using System;
using Microsoft.AspNetCore.Html;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorBuilder
    {
        
        IViewAdaptorBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> action);
        
        IViewAdaptorBuilder AdaptView(string viewName);

        IViewAdaptorBuilder AdaptModel(object model);

    }
    
    public class ViewAdaptorBuilder : IViewAdaptorBuilder
    {

        private readonly IViewAdaptorResult _viewAdaptorResult;

        public IViewAdaptorResult ViewAdaptorResult => _viewAdaptorResult;

        public ViewAdaptorBuilder()
        {
            _viewAdaptorResult = new ViewAdaptorResult();
        }
        
        public IViewAdaptorBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> alteration)
        {
            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            _viewAdaptorResult.OutputAlterations.Add(alteration);
            return this;
        }

        public IViewAdaptorBuilder AdaptView(string viewName)
        {
            if (viewName == null)
            {
                throw new NullReferenceException(nameof(viewName));
            }

            _viewAdaptorResult.ViewAlterations.Add(viewName);
            return this;
        }
        
        public IViewAdaptorBuilder AdaptModel(object model)
        {
            if (model == null)
            {
                throw new NullReferenceException(nameof(model));
            }

            _viewAdaptorResult.ModelAlterations.Add(model);
            return this;
        }

    }
}
