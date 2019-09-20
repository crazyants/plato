using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Assets.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("assets", Attributes = "section")]
    public class AssetsTagHelper : TagHelper
    {

        const string AreaKey = "area";
        const string ControllerKey = "controller";
        const string ActionKey = "action";

        [HtmlAttributeName("section")]
        public AssetSection Section { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

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
         
            // Get provided assets
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
            var matchingEnvironment = environments.FirstOrDefault(g => g.TargetEnvironment == TargetEnvironment.All || g.TargetEnvironment == GetDeploymentMode());

            // Filter by section
            var assets = matchingEnvironment?.Resources.Where(r => r.Section == Section);
          
            // Filter by layout consttaints
            assets = FilterLayoutContraints(assets);
            
            // Filter by route consttaints
            assets = FilterRouteContraints(assets);

            // Return ordered list
            return assets?.OrderBy(p => p.Order).ToList();            

        }

        public IEnumerable<Asset> FilterLayoutContraints(IEnumerable<Asset> assets)
        {

            if (assets == null)
            {
                return null;
            }

            var parts = ViewContext.ExecutingFilePath.Split('/');
            var layout = string.Empty;
            if (parts.Length > 0)
            {
                layout = parts[parts.Length - 1].ToString();
            }
            
            // Our output
            var output = new List<Asset>();
            foreach (var asset in assets)
            {

                // No constaints to test, add the asset and move along
                if (asset.Constraints == null)
                {
                    output.Add(asset);
                    continue;
                }

                // No layout constaint or layout to test, add the asset and move along
                if (string.IsNullOrEmpty(asset.Constraints.Layout) ||
                    string.IsNullOrEmpty(layout))
                {
                    output.Add(asset);
                    continue;
                }

                // Test layout constraint, adding only matching assets
                if (layout.StartsWith(asset.Constraints.Layout, StringComparison.OrdinalIgnoreCase) ||
                    layout.Equals(asset.Constraints.Layout, StringComparison.OrdinalIgnoreCase))
                {
                    output.Add(asset);
                }
                
            }

            return output;

        }
     
        public IEnumerable<Asset> FilterRouteContraints(IEnumerable<Asset> assets)
        {

            if (assets == null)
            {
                return null;
            }

            // Get current area, controller & action

            var routeValues = ViewContext.RouteData.Values;

            var area = string.Empty;
            if (routeValues.ContainsKey(AreaKey))
            {
                area = routeValues[AreaKey].ToString();
            }

            var controller = string.Empty;
            if (routeValues.ContainsKey(ControllerKey))
            {
                controller = routeValues[ControllerKey].ToString();
            }

            var action = string.Empty;
            if (routeValues.ContainsKey(ActionKey))
            {
                action = routeValues[ActionKey].ToString();
            }

            // Our output
            var output = new List<Asset>();
         
            // Test all asset constraints
            foreach (var asset in assets)
            {

                // No constaints to test, add the asset and move along
                if (asset.Constraints == null)
                {
                    output.Add(asset);
                    continue;
                }

                // No route constaints to test, add the asset and move along
                if (asset.Constraints.Routes == null)
                {
                    output.Add(asset);
                    continue;
                }

                var match = false;

                // Test constraints, adding only assets matching 
                // any of the supplied route constraints
                foreach (var contraint in asset.Constraints.Routes)
                {
                    
                    // Test area, controller & action
                    if (contraint.ContainsKey(AreaKey) &&
                        contraint.ContainsKey(ControllerKey) &&
                        contraint.ContainsKey(ActionKey))
                    {
                        match =
                            contraint[AreaKey].Equals(area, StringComparison.OrdinalIgnoreCase) &&
                            contraint[ControllerKey].Equals(controller, StringComparison.OrdinalIgnoreCase) &&
                            contraint[ActionKey].Equals(action, StringComparison.OrdinalIgnoreCase);
                        break;
                    }

                    // Test area & controller
                    if (contraint.ContainsKey(AreaKey) &&
                        contraint.ContainsKey(ControllerKey))
                    {
                        match =
                            contraint[AreaKey].Equals(area, StringComparison.OrdinalIgnoreCase) &&
                            contraint[ControllerKey].Equals(controller, StringComparison.OrdinalIgnoreCase);
                        if (match)
                        {
                            break;
                        }
                    }

                    // Test area
                    if (contraint.ContainsKey(AreaKey))
                    {
                        match = contraint[AreaKey].Equals(area, StringComparison.OrdinalIgnoreCase);
                        if (match)
                        {
                            break;
                        }
                    }
                    
                }

                // Add the asset to the output if any of the constraints matched
                if (match)
                {
                    output.Add(asset);
                }

            }

            return output;

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

                // Iterate through each matching provided environment adding
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
            return new HtmlString($"<script {BuildAttributes(asset)}src=\"{asset.Url}\"></script>");
        }

        IHtmlContent BuildCssInclude(Asset asset)
        {
            return new HtmlString($"<link rel=\"stylesheet\" href=\"{asset.Url}\" {BuildAttributes(asset)}/>");
        }

        IHtmlContent BuildMeta(Asset asset)
        {
            return new HtmlString($"<meta {BuildAttributes(asset)}/>");
        }

        string BuildAttributes(Asset asset)
        {

            if (asset.Attributes == null)
            {
                return string.Empty;
            }

            if (asset.Attributes.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (var attribute in asset.Attributes)
            {
                sb
                    .Append(attribute.Key)
                    .Append("=\"")
                    .Append(attribute.Value)
                    .Append("\" ");
            }

            return sb.ToString();

        }

        #endregion

    }

}
