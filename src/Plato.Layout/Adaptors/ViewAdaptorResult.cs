
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Plato.Layout.Adaptors
{

    public interface IViewAdaptorResult
    {
        string ViewName { get; set; }
        
        IList<Func<IHtmlContent, IHtmlContent>> OutputAlterations { get; set; }
        
        IList<string> ViewAlterations { get; set; }

        IList<object> ModelAlterations { get; set; }

        IViewAdaptorResult For(string viewName);
    }

    public class ViewAdaptorResult : IViewAdaptorResult
    {

        private IList<Func<IHtmlContent, IHtmlContent>> _outputAlterations;
        private IList<string> _viewAlterations;
        private IList<object> _modelAlterations;

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

        public IList<object> ModelAlterations
        {
            get => _modelAlterations ?? (_modelAlterations = new List<object>());
            set => _modelAlterations = value;
        }

        public IViewAdaptorResult For(string viewName)
        {
            this.ViewName = viewName;
            return this;
        }
    }
}
