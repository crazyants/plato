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
                foreach (var resource in resources)
                {
                    switch (resource.Type)
                    {

                        case ResourceType.JavaScript:
                            
                            sw.Write(BuildJavaScriptInclude(resource));
                            sw.Write(sw.NewLine);
                            break;

                        case ResourceType.Css:

                            sw.Write(BuildCssInclude(resource));
                            sw.Write(sw.NewLine);
                            break;

                        case ResourceType.Meta:

                            sw.Write(BuildMeta(resource));
                            sw.Write(sw.NewLine);
                            break;

                    }
                }
            }

            var sb = sw.GetStringBuilder();
            output.Content.SetHtmlContent(sb.ToString());
            
        }

        #endregion

        #region "Private Methods"

        Environment GetDeploymentMode()
        {
            if (_hostingEnvironment.IsProduction())
                return Environment.Production;
            if (_hostingEnvironment.IsStaging())
                return Environment.Staging;
            return Environment.Development;
        }

        async Task<IList<Resource>> GetResourcesAsync()
        {
            var groups = await GetMergedResourcesAsync();
            var group = groups.FirstOrDefault(g => g.Environment == GetDeploymentMode());
            return @group?.Resources.Where(r => r.Section == Section).ToList();
        }

        async Task<IEnumerable<ResourceGroup>> GetMergedResourcesAsync()
        {
            
            var provided = await _resourceManager.GetResources();
            var defaults = DefaultResources.GetDefaultResources();
            var defaultGroups = defaults.ToList();
            var providedGroups = provided.ToList();

            // Merge resources in provided groups with resources in default groups
            var dict = defaultGroups.ToDictionary(p => p.Environment);
            foreach (var defaultGroup in defaultGroups)
            {

                var matchingGroups = providedGroups
                    .Where(g => g.Environment == defaultGroup.Environment)
                    .ToList();
             
                foreach (var group in matchingGroups)
                {
                    foreach (var resource in group.Resources)
                    {
                        dict[defaultGroup.Environment].Resources.Add(resource);
                    }
                
                }
                
            }

            return dict.Values.ToList();

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
