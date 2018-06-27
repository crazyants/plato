using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Plato.Internal.Resources;
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
            IResourceManager resourceManager, IHostingEnvironment hostingEnvironment)
        {
            _resourceManager = resourceManager;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion


        #region "Implementation"

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;

            var sw = new StringWriter();
         
            var resources = GetResources();
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

            return Task.CompletedTask;

        }

        #endregion

        #region "Private Methods"

        DeploymentMode GetDeploymentMode()
        {
            if (_hostingEnvironment.IsProduction())
                return DeploymentMode.Production;
            if (_hostingEnvironment.IsStaging())
                return DeploymentMode.Staging;
            return DeploymentMode.Development;
        }

        IList<Resource> GetResources()
        {
            var groups = DefaultResources.GetDefaultResources().ToList();
            var group = groups.FirstOrDefault(g => g.DeploymentMode == GetDeploymentMode());
            return @group?.Resources.Where(r => r.Section == Section).ToList();
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
