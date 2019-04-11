using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Categories.ViewModels;
using Plato.Ideas.Categories.Models;
using Plato.Ideas.Categories.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Ideas.Categories.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<CategoryAdmin>
    {
        
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly ICategoryManager<Category> _categoryManager;
        private readonly IFeatureFacade _featureFacade;

        public IStringLocalizer S { get; }


        public AdminViewProvider(
            IStringLocalizer stringLocalizer,
            IContextFacade contextFacade,
            ICategoryStore<Category> categoryStore,
            ICategoryManager<Category> categoryManager,
            IFeatureFacade featureFacade)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;

            S = stringLocalizer;
            _featureFacade = featureFacade;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(CategoryAdmin categoryBase, IViewProviderContext updater)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas.Categories");
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id 'Plato.Ideas.Categories'");
            }

            var viewModel = new CategoryIndexViewModel()
            {
                Options = new CategoryIndexOptions()
                {
                    FeatureId = feature.Id,
                    ChannelId = categoryBase?.Id ?? 0,
                    EnableEdit = true
                }
            };

            return Views(
                View<CategoryBase>("Admin.Index.Header", model => categoryBase).Zone("header").Order(1),
                View<CategoryIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<CategoryIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(CategoryAdmin viewModel, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(CategoryAdmin categoryBase, IViewProviderContext updater)
        {

            var defaultIcons = new DefaultIcons();

            EditCategoryViewModel editCategoryViewModel = null;
            if (categoryBase.Id == 0)
            {
                editCategoryViewModel = new EditCategoryViewModel()
                {
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    IsNewCategory = true,
                    ParentId = categoryBase.ParentId,
                    AvailableChannels = await GetAvailableChannels()
                };
            }
            else
            {
                editCategoryViewModel = new EditCategoryViewModel()
                {
                    Id = categoryBase.Id,
                    ParentId = categoryBase.ParentId,
                    Name = categoryBase.Name,
                    Description = categoryBase.Description,
                    ForeColor = categoryBase.ForeColor,
                    BackColor = categoryBase.BackColor,
                    IconCss = categoryBase.IconCss,
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    AvailableChannels = await GetAvailableChannels()
                };
            }
            
            return Views(
                View<EditCategoryViewModel>("Admin.Edit.Header", model => editCategoryViewModel).Zone("header").Order(1),
                View<EditCategoryViewModel>("Admin.Edit.Content", model => editCategoryViewModel).Zone("content").Order(1),
                View<EditCategoryViewModel>("Admin.Edit.Actions", model => editCategoryViewModel).Zone("actions").Order(1),
                View<EditCategoryViewModel>("Admin.Edit.Footer", model => editCategoryViewModel).Zone("footer").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(CategoryAdmin categoryBase, IViewProviderContext context)
        {

            var model = new EditCategoryViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(categoryBase, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
  
            if (context.Updater.ModelState.IsValid)
            {

                var iconCss = model.IconCss;
                if (!string.IsNullOrEmpty(iconCss))
                {
                    iconCss = model.IconPrefix + iconCss;
                }

                var result = await _categoryManager.UpdateAsync(new Category()
                {
                    Id = categoryBase.Id,
                    FeatureId = categoryBase.FeatureId,
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor,
                    IconCss = iconCss,
                    SortOrder = categoryBase.SortOrder
                });

                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(categoryBase, context);
            
        }

        #endregion

        #region "Private Methods"

        async Task<IEnumerable<SelectListItem>> GetAvailableChannels()
        {

            var output = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = S["root channel"],
                    Value = "0"
                }
            };

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Ideas.Categories");
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id 'Plato.Ideas.Categories'");
            }

            var channels = await _categoryStore.GetByFeatureIdAsync(feature.Id);
            if (channels != null)
            {
                var items = await RecurseChannels(channels);
                foreach (var item in items)
                {
                    output.Add(item);
                }
            }
          
            return output;

        }

        Task<IList<SelectListItem>> RecurseChannels(
            IEnumerable<ICategory> input,
            IList<SelectListItem> output = null,
            int id = 0)
        {

            if (output == null)
            {
                output = new List<SelectListItem>();
            }
            
            var categories = input.ToList();
            foreach (var category in categories)
            {
                if (category.ParentId == id)
                {
                    var indent = "-".Repeat(category.Depth);
                    if (!string.IsNullOrEmpty(indent))
                    {
                        indent += " ";
                    }
                    output.Add(new SelectListItem
                    {
                        Text = indent + category.Name,
                        Value = category.Id.ToString()
                    });
                    RecurseChannels(categories, output, category.Id);
                }
            }

            return Task.FromResult(output);

        }
        
        #endregion

    }
}
