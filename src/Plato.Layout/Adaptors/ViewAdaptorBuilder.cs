using System;
using Microsoft.AspNetCore.Html;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorBuilder
    {
   
        string ViewName { get; }

        IViewAdaptorBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> action);
        
        IViewAdaptorBuilder AdaptView(string viewName);

        IViewAdaptorBuilder AdaptView(string[] viewNames);
        
        IViewAdaptorBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class;

    }
    
    public class ViewAdaptorBuilder : IViewAdaptorBuilder
    {
        public string ViewName { get; }

        private readonly IViewAdaptorResult _viewAdaptorResult;

        public IViewAdaptorResult ViewAdaptorResult => _viewAdaptorResult;

        public ViewAdaptorBuilder(string viewName) 
        {
            _viewAdaptorResult = new ViewAdaptorResult();
            ViewName = viewName;
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

        public IViewAdaptorBuilder AdaptView(string[] viewNames)
        {
            foreach (var viewName in viewNames)
            {
                AdaptView(viewName);
            }
            return this;
        }

        public IViewAdaptorBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class
        {
            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            // wrapper to convert delegates generic argument type
            // to concrete type (object) for storage within adaptor result
            var typedDelegate = new Func<object, object>((object input) => alteration((TModel)input));

            _viewAdaptorResult.ModelAlterations.Add(typedDelegate);
            return this;
        }

    }
}
