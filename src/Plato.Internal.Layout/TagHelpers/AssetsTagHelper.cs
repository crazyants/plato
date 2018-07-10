using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("assets")]
    public class AssetsTagHelper : TagHelper
    {
      
        public AssetSection Section { get; set; }

        #region "Constrcutor"

        private readonly IAssetManager _assetManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public AssetsTagHelper(
            IAssetManager assetManager,
            IHostingEnvironment hostingEnvironment)
        {
            _assetManager = assetManager;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion
        
        #region "Implementation"

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;

            var sw = new StringWriter();
         
            var assets = await GetAssetsAsync();
            if (assets != null)
            {
                var i = 1;
                foreach (var asset in assets)
                {
                    switch (asset.Type)
                    {

                        case AssetType.IncludeJavaScript:
                            
                            sw.Write(BuildJavaScriptInclude(asset));
                            if (i < assets.Count)
                            {
                                sw.Write(sw.NewLine);
                            }
                            break;

                        case AssetType.IncludeCss:

                            sw.Write(BuildCssInclude(asset));
                            if (i < assets.Count)
                            {
                                sw.Write(sw.NewLine);
                            }
                            break;

                        case AssetType.Meta:

                            sw.Write(BuildMeta(asset));
                            if (i < assets.Count)
                            {
                                sw.Write(sw.NewLine);
                            }
                            break;

                        case AssetType.InlineJavaScript:

                            sw.Write(BuildInlineJavaScript(asset));
                            if (i < assets.Count)
                            {
                                sw.Write(sw.NewLine);
                            }
                            break;


                    }

                    i++;
                }
            }

            var sb = sw.GetStringBuilder();
            output.Content.SetHtmlContent(sb.ToString());
            
        }

        #endregion

        #region "Private Methods"
        
        // Get all resources matching environment and section
        async Task<IList<Asset>> GetAssetsAsync()
        {

            // Get all default and provided environments
            var environments = await GetMergedEnvironmentsAsync();

            // Filter by environment
            var matchingEnvironments = environments.FirstOrDefault(g => g.TargetEnvironment == GetDeploymentMode());

            // filter by section and return ordered assets
            return @matchingEnvironments?.Resources
                .Where(r => r.Section == Section)
                .OrderBy(p => p.Priority)
                .ToList();

        }

        async Task<IEnumerable<AssetEnvironment>> GetMergedEnvironmentsAsync()
        {
            
            // Get provided resources
            var provided = await _assetManager.GetAssets();
            var providedEnvironments = provided.ToList();

            // Get default resources
            var defaults = DefaultAssets.GetDefaultResources();
            var defaultEnvironments = defaults.ToList();

            // Merge provided resources into default groups
            var output = defaultEnvironments.ToDictionary(p => p.TargetEnvironment);
            foreach (var defaultEnvironment in defaultEnvironments)
            {

                // Get provided resources matching our current environment
                var matchingEnvironments = providedEnvironments
                    .Where(g => g.TargetEnvironment == defaultEnvironment.TargetEnvironment)
                    .ToList();

                // Interate through each matching provided environment adding
                // resources from that environment into our default environments
                foreach (var group in matchingEnvironments)
                {
                    foreach (var resource in group.Resources)
                    {
                        output[defaultEnvironment.TargetEnvironment].Resources.Add(resource);
                    }
                }
                
            }

            return output.Values.ToList();

        }
        
        TargetEnvironment GetDeploymentMode()
        {
            if (_hostingEnvironment.IsProduction())
                return TargetEnvironment.Production;
            if (_hostingEnvironment.IsStaging())
                return TargetEnvironment.Staging;
            return TargetEnvironment.Development;
        }
        
        IHtmlContent BuildJavaScriptInclude(Asset asset)
        {
            return new HtmlString($"<script src=\"{asset.Url}\"></script>");
        }

        IHtmlContent BuildCssInclude(Asset asset)
        {
            return new HtmlString($"<link rel=\"stylesheet\" href=\"{asset.Url}\" />");
        }

        IHtmlContent BuildMeta(Asset asset)
        {
            return new HtmlString($"<script src=\"{asset.Url}\"></script>");
        }

        IHtmlContent BuildInlineJavaScript(Asset asset)
        {
            var builder = new HtmlContentBuilder();
            var content = builder
                    .AppendHtml("<script>")
                    .AppendHtml(System.Environment.NewLine)
                    .AppendHtml(asset.InlineContent)
                    .AppendHtml(System.Environment.NewLine)
                    .AppendHtml("</script>");

            return content.ToHtmlString();
        }


        #endregion

    }

}
