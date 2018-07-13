using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
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

            // Register color picker scripts with asset manager
        
            if (!String.IsNullOrEmpty(Color))
            {
                output.Attributes.SetAttribute("autofocus", "");
            }

            return Task.CompletedTask;

        }

        #endregion

    }
}
