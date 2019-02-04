using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.ViewAdapters
{
    
    public class ViewAdapterBuilder : IViewAdapterBuilder
    {
        public string ViewName { get; }

        private readonly IViewAdapterResult _viewAdapterResult;

        public IViewAdapterResult ViewAdapterResult => _viewAdapterResult;

        public ViewAdapterBuilder(string viewName) 
        {
            _viewAdapterResult = new ViewAdapterResult();
            ViewName = viewName;
        }
        
        public IViewAdapterBuilder AdaptOutput(Func<IHtmlContent, IHtmlContent> alteration)
        {
            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }

            _viewAdapterResult.OutputAlterations.Add(alteration);
            return this;
        }

        public IViewAdapterBuilder AdaptView(string viewName)
        {
            if (viewName == null)
            {
                throw new NullReferenceException(nameof(viewName));
            }

            _viewAdapterResult.ViewAlterations.Add(viewName);
            return this;
        }

        public IViewAdapterBuilder AdaptView(string[] viewNames)
        {
            foreach (var viewName in viewNames)
            {
                AdaptView(viewName);
            }
            return this;
        }

        public IViewAdapterBuilder AdaptModel<TModel>(Func<TModel, object> alteration) where TModel : class
        {
            if (alteration == null)
            {
                throw new NullReferenceException(nameof(alteration));
            }
            
            // wrapper to convert delegates generic argument type
            // to concrete type (object) for storage within adapter result
            var typedDelegate = new Func<object, object>((object input) =>
            {

                // use first argument from anonymous type as model
                // todo: implement support for multiple arguments within anonymous types
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

            _viewAdapterResult.ModelAlterations.Add(typedDelegate);
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
