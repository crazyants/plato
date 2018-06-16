using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Layout.Views;

namespace Plato.Internal.Layout.TagHelpers
{
    
    [HtmlTargetElement("view")]
    public class ViewTagHelper : TagHelper
    {

        public dynamic Model { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }
        
        private readonly IViewHelperFactory _viewHelperFactory;

        private IViewDisplayHelper _viewDisplayHelper;

        public ViewTagHelper(
            IViewHelperFactory viewHelperFactory)
        {
            _viewHelperFactory = viewHelperFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (this.Model == null)
            {
                throw new ArgumentNullException(nameof(this.Model));
            }

            var builder = await Build();
            if (builder == null)
            {
                throw new Exception($"An error occurred whilst attempting to activate a view. The supplied model of type {Model} is not a valid type. The supplied model must implement the IView interface.");
            }

            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(builder);

        }
        
        void EnsureViewHelper()
        {
            if (_viewDisplayHelper == null)
            {
                _viewDisplayHelper = _viewHelperFactory.CreateHelper(ViewContext);
            }
        }

        private async Task<IHtmlContent> Build()
        {

            EnsureViewHelper();

            HtmlContentBuilder builder = null;
            
            if (this.Model is IEnumerable<IView>)
            {
                builder = new HtmlContentBuilder();
                foreach (var view in Model)
                {
                    builder.AppendHtml(await InvokeView(view));
                }
            }
            else
            {
                if (this.Model is IView)
                {
                    builder = new HtmlContentBuilder();
                    builder.AppendHtml(await InvokeView(this.Model));
                }
            }
       
            return builder;

        }
        
        async Task<IHtmlContent> InvokeView(IView view)
        {
            return await _viewDisplayHelper.DisplayAsync(view);
        }
    }
}
