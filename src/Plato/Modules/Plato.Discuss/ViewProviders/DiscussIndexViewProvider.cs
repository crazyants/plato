using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Discuss.ViewProviders
{
    public class DiscussIndexViewProvider : BaseViewProvider<DiscussIndexViewModel>
    {

        private readonly IEntityStore<Entity> _entityStore;


        public DiscussIndexViewProvider(
            IEntityStore<Entity> entityStore)
        {
            _entityStore = entityStore;
        }



        public override Task<IViewProviderResult> BuildDisplayAsync(DiscussIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        public override Task<IViewProviderResult> BuildIndexAsync(DiscussIndexViewModel viewModel, IUpdateModel updater)
        {
            return Task.FromResult(
                Views(
                    View<DiscussIndexViewModel>("Discuss.Index.Header", model => viewModel).Zone("header"),
                    View<DiscussIndexViewModel>("Discuss.Index.Tools", model => viewModel).Zone("tools"),
                    View<DiscussIndexViewModel>("Discuss.Index.Sidebar", model => viewModel).Zone("sidebar"),
                    View<DiscussIndexViewModel>("Discuss.Index.Content", model => viewModel).Zone("content")
                ));
        }

        public override Task<IViewProviderResult> BuildEditAsync(DiscussIndexViewModel model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(DiscussIndexViewModel indexViewModel, IUpdateModel updater)
        {

            var model = new DiscussIndexViewModel();;

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildIndexAsync(indexViewModel, updater);
            }

            if (updater.ModelState.IsValid)
            {


                var entity = new Entity();
                entity.Title = indexViewModel.NewEntityViewModel.Title?.Trim();
                entity.Markdown = indexViewModel.NewEntityViewModel.Message?.Trim();


                var newTopic = await _entityStore.CreateAsync(entity);


                //var result = await _userManager.UpdateAsync(user);

                //foreach (var error in result.Errors)
                //{
                //    updater.ModelState.AddModelError(string.Empty, error.Description);
                //}

            }

            return await BuildIndexAsync(indexViewModel, updater);
            

        }
    }

}
