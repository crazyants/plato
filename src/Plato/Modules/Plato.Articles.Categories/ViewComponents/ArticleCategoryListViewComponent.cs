using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Stores;
using Plato.Articles.Categories.Models;
using Plato.Categories.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Models.Features;

namespace Plato.Articles.Categories.ViewComponents
{

    public class ArticleCategoryListViewComponent : ViewComponent
    {
        private readonly ICategoryStore<CategoryHome> _channelStore;
       
        public ArticleCategoryListViewComponent(ICategoryStore<CategoryHome> channelStore)
        {
            _channelStore = channelStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(CategoryIndexOptions options)
        {

            if (options == null)
            {
                options = new CategoryIndexOptions();
            }
            
            return View(await GetIndexModel(options));

        }
        
        async Task<CategoryListViewModel<CategoryHome>> GetIndexModel(CategoryIndexOptions options)
        {
            var categories = await _channelStore.GetByFeatureIdAsync(options.FeatureId);
            return new CategoryListViewModel<CategoryHome>()
            {
                Options = options,
                Channels = categories?.Where(c => c.ParentId == options.ChannelId)
            };
        }

    }


}
