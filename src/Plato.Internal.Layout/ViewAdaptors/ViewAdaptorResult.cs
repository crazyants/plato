using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace Plato.Internal.Layout.ViewAdaptors
{
    
    public interface IViewAdaptorResult
    {

        IViewAdaptorBuilder Builder { get; set; }

        IList<Func<IHtmlContent, IHtmlContent>> OutputAlterations { get; set; }
        
        IList<string> ViewAlterations { get; set; }

        IList<Func<object, object>> ModelAlterations { get; set; }

    }

    public class ViewAdaptorResult : IViewAdaptorResult 
    {

        private IList<Func<IHtmlContent, IHtmlContent>> _outputAlterations;
        private IList<string> _viewAlterations;
        private IList<Func<object, object>> _modelAlterations;
        
        public IViewAdaptorBuilder Builder { get; set; }

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
   

    }
}
