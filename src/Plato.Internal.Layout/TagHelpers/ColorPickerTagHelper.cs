using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Assets.Abstractions;
using Plato.Internal.Scripting.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    // https://github.com/farbelous/bootstrap-colorpicker

    [HtmlTargetElement(Attributes = "asp-color-picker")]
    public class ColorPickerTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-color-picker")]
        public string Color { get; set; }

        private readonly IScriptManager _scriptManager;

        public ColorPickerTagHelper(
            IAssetManager assetManager,
            IScriptManager scriptManager)
        {

            _scriptManager = scriptManager;
            
            // Register JavasScript and CSSS with asset manager
            assetManager.SetAssets(new List<AssetEnvironment>
            {
                new AssetEnvironment(TargetEnvironment.Development,
                    new List<Asset>()
                    {
                        new Asset()
                        {
                            Url = "/css/vendors/bootstrap-colorpicker.css",
                            Type = AssetType.IncludeCss,
                            Section = AssetSection.Header
                        },
                        new Asset()
                        {
                            Url = "/js/vendors/bootstrap-colorpicker.js",
                            Type = AssetType.IncludeJavaScript,
                            Section = AssetSection.Footer
                        }
                    })
            });

        }
        
        #region "Implementation"

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var script = "$('[data-provide=\"color-picker\"]').colorpicker( { format: \"hex\", align: \"left\" });";
            
            _scriptManager.RegisterScriptBlock(
                new ScriptBlock(new HtmlString(script)),
                ScriptSection.Footer);

            output.Attributes.SetAttribute("data-provide", "color-picker");
            return Task.CompletedTask;
        }

        #endregion

    }
}
