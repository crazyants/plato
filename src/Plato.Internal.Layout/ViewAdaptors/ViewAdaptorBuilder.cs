using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.ViewAdaptors
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
            var typedDelegate = new Func<object, object>((object input) =>
            {

                // use first argument from anonymous type as mdoel
                // todo: implement support for mulitple arguments within anonymous types
                if (IsViewModelAnonymousType(input))
                {
                    var args = new List<object>();
                    var properties = TypeDescriptor.GetProperties(input);
                    foreach (PropertyDescriptor property in properties)
                    {
                        args.Add(property.GetValue(input));
                    }
                    return alteration((TModel)args[0]);
                }

                return alteration((TModel) input);

            });

            _viewAdaptorResult.ModelAlterations.Add(typedDelegate);
            return this;
        }
        
        bool IsViewModelAnonymousType(object model)
        {

            // We need a model to inspect
            if (model == null)
            {
                return false;
            }

            return model
                .GetType()
                .GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any();

        }

    }
}
