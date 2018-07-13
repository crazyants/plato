using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    // https://github.com/farbelous/bootstrap-colorpicker

    [HtmlTargetElement(Attributes = "asp-color-picker")]
    public class ColorPickerTagHelper : TagHelper
    {

        [HtmlAttributeName("asp-color-picker")]
        public string Color { get; set; }
        

        public ColorPickerTagHelper(IAssetManager assetManager)
        {

            var script = "$('[data-provide=\"color-picker\"]').colorpicker( {color: \"{color}\", format: \"hex\", align: \"left\" });";
            script = script.Replace("{color}", this.Color);

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
                        },
                        new Asset()
                        {
                            InlineContent = new HtmlString(script),
                            Type = AssetType.InlineJavaScript,
                            Section = AssetSection.Footer,
                            Priority = 99999,
                        }
                    })
            });

        }
        
        #region "Implementation"

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("data-provide", "color-picker");
            return Task.CompletedTask;
        }

        #endregion

    }
}
