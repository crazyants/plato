using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Categories.Stores;
using Plato.Discuss.Channels.Models;
using Plato.Discuss.Channels.ViewModels;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Shell;

namespace Plato.Discuss.Channels.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<CategoryBase>
    {
        
        private readonly IContextFacade _contextFacade;
        private readonly ICategoryStore<Channel> _categoryStore;
        private readonly ICategoryManager<Channel> _categoryManager;

        public IStringLocalizer S { get; }


        public AdminViewProvider(
            IContextFacade contextFacade,
            ICategoryStore<Channel> categoryStore,
            ICategoryManager<Channel> categoryManager,
            IStringLocalizer<AdminViewProvider> stringLocalizer)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _categoryManager = categoryManager;

            S = stringLocalizer;

        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(CategoryBase category, IUpdateModel updater)
        {
            //var indexViewModel = await GetIndexModel(category?.Id ?? 0);

            var viewModel = new ChannelIndexViewModel()
            {
                ChannelViewOpts = new ViewOptions()
                {
                    ChannelId = category?.Id ?? 0,
                    EnableEdit = true
                }
            };

            return Task.FromResult(Views(
                View<CategoryBase>("Admin.Index.Header", model => category).Zone("header").Order(1),
                View<ChannelIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<ChannelIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(CategoryBase viewModel, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildEditAsync(CategoryBase categoryBase, IUpdateModel updater)
        {

            var defaultIcons = new DefaultIcons();

            EditChannelViewModel editChannelViewModel = null;
            if (categoryBase.Id == 0)
            {
                editChannelViewModel = new EditChannelViewModel()
                {
                    IconPrefix = defaultIcons.Prefix,
                    ChannelIcons = defaultIcons,
                    IsNewChannel = true,
                    AvailableChannels = await GetAvailableChannels()
                };
            }
            else
            {
                editChannelViewModel = new EditChannelViewModel()
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
                View<EditChannelViewModel>("Admin.Edit.Header", model => editChannelViewModel).Zone("header").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Content", model => editChannelViewModel).Zone("content").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Actions", model => editChannelViewModel).Zone("actions").Order(1),
                View<EditChannelViewModel>("Admin.Edit.Footer", model => editChannelViewModel).Zone("footer").Order(1)
            );
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(CategoryBase categoryBase, IUpdateModel updater)
        {

            var model = new EditChannelViewModel();

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(categoryBase, updater);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
  
            //Category category = null;

            if (updater.ModelState.IsValid)
            {

                var iconCss = model.IconCss;
                if (!string.IsNullOrEmpty(iconCss))
                {
                    iconCss = model.IconPrefix + iconCss;
                }

                var result = await _categoryManager.UpdateAsync(new Channel()
                {
                    Id = categoryBase.Id,
                    FeatureId = categoryBase.FeatureId,
                    ParentId = model.ParentId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor,
                    IconCss = iconCss
                });

                foreach (var error in result.Errors)
                {
                    updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(categoryBase, updater);


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
            
            var feature = await GetcurrentFeature();
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
        
        //async Task<ChannelListViewModel> GetIndexModel(int parentId)
        //{
        //    var feature = await GetcurrentFeature();
        //    var categories = await _categoryStore.GetByFeatureIdAsync(feature.Id);
        //    return new ChannelListViewModel()
        //    {
        //        Channels = categories?.Where(c => c.ParentId == parentId)
        //    };
        //}
        
        async Task<ShellModule> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Channels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the Id '{featureId}'");
            }
            return feature;
        }

        #endregion

    }
}
