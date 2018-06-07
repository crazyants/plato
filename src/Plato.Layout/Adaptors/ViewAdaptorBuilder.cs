using System;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.Language.Intermediate;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorBuilder
    {
        
        IViewAdaptorBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> action);
        
        IViewAdaptorBuilder AdaptView(string viewName);

        IViewAdaptorBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class;

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
        
        public IViewAdaptorBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class
        {
            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            var typedDeleate = new Func<object, object>((object input) =>
            {
                return alteration((TModel)input);
            });

            _viewAdaptorResult.ModelAlterations.Add(typedDeleate);
            return this;
        }

    }
}
