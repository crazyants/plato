using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Resources.Abstractions;

namespace Plato.Internal.Layout.TagHelpers
{

    [HtmlTargetElement("resources")]
    public class ResourcesTagHelper : TagHelper
    {
      
        public ResourceSection Section { get; set; }

        #region "Constrcutor"

        private readonly IResourceManager _resourceManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ResourcesTagHelper(
            IResourceManager resourceManager,
            IHostingEnvironment hostingEnvironment)
        {
            _resourceManager = resourceManager;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion
        
        #region "Implementation"

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;

            var sw = new StringWriter();
         
            var resources = await GetResourcesAsync();
            if (resources != null)
            {
                var i = 1;
                foreach (var resource in resources)
                {
                    switch (resource.Type)
                    {

                        case ResourceType.JavaScript:
                            
                            sw.Write(BuildJavaScriptInclude(resource));
                            if (i < resources.Count)
                            {
                                sw.Write(sw.NewLine);
                            }
                            break;

                        case ResourceType.Css:

                            sw.Write(BuildCssInclude(resource));
                            if (i < resources.Count)
                            {
                                sw.Write(sw.NewLine);
                            }
                            break;

                        case ResourceType.Meta:

                            sw.Write(BuildMeta(resource));
                            if (i < resources.Count)
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
        async Task<IList<Resource>> GetResourcesAsync()
        {

            // Get all default and provided environments
            var environments = await GetMergedEnvironmentsAsync();

            // Filter by environment
            var matchingEnvironments = environments.FirstOrDefault(g => g.Environment == GetDeploymentMode());

            // filter by section
            return @matchingEnvironments?.Resources.Where(r => r.Section == Section).ToList();

        }

        async Task<IEnumerable<ResourceEnvironment>> GetMergedEnvironmentsAsync()
        {
            
            // Get provided resources
            var provided = await _resourceManager.GetResources();
            var providedEnvironments = provided.ToList();

            // Get default resources
            var defaults = DefaultResources.GetDefaultResources();
            var defaultEnvironments = defaults.ToList();

            // Merge provided resources into default groups
            var output = defaultEnvironments.ToDictionary(p => p.Environment);
            foreach (var defaultEnvironment in defaultEnvironments)
            {

                // Get provided resources matching our current environment
                var matchingEnvironments = providedEnvironments
                    .Where(g => g.Environment == defaultEnvironment.Environment)
                    .ToList();

                // Interate through each matching provided environment adding
                // resources from that environment into our default environments
                foreach (var group in matchingEnvironments)
                {
                    foreach (var resource in group.Resources)
                    {
                        output[defaultEnvironment.Environment].Resources.Add(resource);
                    }
                }
                
            }

            return output.Values.ToList();

        }
        
        Environment GetDeploymentMode()
        {
            if (_hostingEnvironment.IsProduction())
                return Environment.Production;
            if (_hostingEnvironment.IsStaging())
                return Environment.Staging;
            return Environment.Development;
        }
        
        IHtmlContent BuildJavaScriptInclude(Resource resource)
        {
            return new HtmlString($"<script src=\"{resource.Url}\"></script>");
        }

        IHtmlContent BuildCssInclude(Resource resource)
        {
            return new HtmlString($"<link rel=\"stylesheet\" href=\"{resource.Url}\" />");
        }

        IHtmlContent BuildMeta(Resource resource)
        {
            return new HtmlString($"<script src=\"{resource.Url}\"></script>");
        }

        #endregion
        
    }

}
