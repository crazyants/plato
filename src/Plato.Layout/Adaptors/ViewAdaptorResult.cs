using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Plato.Layout.Adaptors
{

    public class TypedModelAlterations<TModel> where TModel : class
    {

        private IList<Func<TModel, TModel>> _modelAlterations;

        public IList<Func<TModel, TModel>> ModelAlterations
        {
            get => _modelAlterations ?? (_modelAlterations = new List<Func<TModel, TModel>>());
            set => _modelAlterations = value;
        }

        public void AddMode(TModel model)
        {

        }

    }

    public interface IViewAdaptorResult
    {
        string ViewName { get; set; }
        
        IList<Func<IHtmlContent, IHtmlContent>> OutputAlterations { get; set; }
        
        IList<string> ViewAlterations { get; set; }

        IList<Func<object, object>> ModelAlterations { get; set; }

        IViewAdaptorResult For(string viewName);
    }

    public class ViewAdaptorResult : IViewAdaptorResult 
    {

        private IList<Func<IHtmlContent, IHtmlContent>> _outputAlterations;
        private IList<string> _viewAlterations;
        private IList<Func<object, object>> _modelAlterations;

        public string ViewName { get; set; }

        public IViewAdaptorBuilder AdaptorBuilder { get; set; }

        public IList<Func<IHtmlContent, IHtmlContent>> OutputAlterations
        {
            get => _outputAlterations ?? (_outputAlterations = new List<Func<IHtmlContent, IHtmlContent>>());
            set => _outputAlterations = value;
        }

        public IList<string> ViewAlterations
        {
            get => _viewAlterations ?? (_viewAlterations = new List<string>());
            set => _viewAlterations = value;
        }

        public IList<Func<object, object>> ModelAlterations
        {
            get => _modelAlterations ?? (_modelAlterations = new List<Func<object, object>>());
            set => _modelAlterations = value;
        }
   
        public IViewAdaptorResult For(string viewName) 
        {
            this.ViewName = viewName;
            return this;
        }

    }
}
