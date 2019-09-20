using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{
    
    [HtmlTargetElement(Attributes = "asp-date-picker")]
    public class DatePickerTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-date-picker")]
        public string Color { get; set; }

        private readonly IScriptManager _scriptManager;

        public DatePickerTagHelper(
            IAssetManager assetManager,
            IScriptManager scriptManager)
        {

            _scriptManager = scriptManager;

            // Register JavasScript and CSS with asset manager
            assetManager.SetAssets(new List<AssetEnvironment>
            {
                new AssetEnvironment(TargetEnvironment.Development,
                    new List<Asset>()
                    {
                        new Asset()
                        {
                            Url = "/css/vendors/bootstrap-datepicker.css",
                            Type = AssetType.IncludeCss,
                            Section = AssetSection.Header
                        },
                        new Asset()
                        {
                            Url = "/js/vendors/bootstrap-datepicker.js",
                            Type = AssetType.IncludeJavaScript,
                            Section = AssetSection.Footer
                        }
                    }),
                  new AssetEnvironment(TargetEnvironment.Staging,
                    new List<Asset>()
                    {
                        new Asset()
                        {
                            Url = "/css/vendors/bootstrap-datepicker.css",
                            Type = AssetType.IncludeCss,
                            Section = AssetSection.Header
                        },
                        new Asset()
                        {
                            Url = "/js/vendors/bootstrap-datepicker.js",
                            Type = AssetType.IncludeJavaScript,
                            Section = AssetSection.Footer
                        }
                    }),
                    new AssetEnvironment(TargetEnvironment.Production,
                    new List<Asset>()
                    {
                        new Asset()
                        {
                            Url = "/css/vendors/bootstrap-datepicker.css",
                            Type = AssetType.IncludeCss,
                            Section = AssetSection.Header
                        },
                        new Asset()
                        {
                            Url = "/js/vendors/bootstrap-datepicker.js",
                            Type = AssetType.IncludeJavaScript,
                            Section = AssetSection.Footer
                        }
                    })
            });

        }
        
        #region "Implementation"

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var script = "$('[data-provide=\"date-picker\"]').datepicker();";
            
            _scriptManager.RegisterScriptBlock(
                new ScriptBlock(new HtmlString(script)),
                ScriptSection.Footer);

            output.Attributes.SetAttribute("data-provide", "date-picker");
            return Task.CompletedTask;
        }

        #endregion

    }
}
