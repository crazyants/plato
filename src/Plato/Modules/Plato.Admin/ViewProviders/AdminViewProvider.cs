using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Admin.Models;
using Plato.Admin.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Admin.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<AdminIndex>
    {
        private readonly IShellDescriptorManager _shellDescriptorManager;

        public AdminViewProvider(IShellDescriptorManager shellDescriptorManager)
        {
            _shellDescriptorManager = shellDescriptorManager;
        }
        
        public override async Task<IViewProviderResult> BuildIndexAsync(AdminIndex viewModel, IViewProviderContext context)
        {

            var adminViewModel = new AdminViewModel()
            {
                FeatureCategories = await GetAvailableFeatureCategoriesAsync()
            };

            return Views(
                View<AdminViewModel>("Admin.Index.Header", model => adminViewModel).Zone("header").Order(1),
                View<AdminViewModel>("Admin.Index.Tools", model => adminViewModel).Zone("tools").Order(1),
                View<AdminViewModel>("Admin.Index.Content", model => adminViewModel).Zone("content").Order(1)
            );
            
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(AdminIndex viewModel, IViewProviderContext context)
        {
            return await BuildEditAsync(viewModel, context);
        }


        async Task<IEnumerable<SelectListItem>> GetAvailableFeatureCategoriesAsync()
        {

            var output = new List<SelectListItem>();

            var features = await _shellDescriptorManager.GetFeaturesAsync();
            foreach (var feature in features.GroupBy(f => f.Descriptor.Category).OrderBy(o => o.Key))
            {
                output.Add(new SelectListItem
                {
                    Text = feature.Key,
                    Value = feature.Key
                });
            }

            return output;
        }


    }

}
