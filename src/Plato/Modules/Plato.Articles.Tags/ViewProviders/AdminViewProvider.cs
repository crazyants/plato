using System;
using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Tags.Services;
using Plato.Tags.Stores;
using Plato.Tags.ViewModels;
using Plato.Articles.Tags.Models;
using Plato.Articles.Tags.ViewModels;

namespace Plato.Articles.Tags.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<TagAdmin>
    {


        private readonly ITagManager<Tag> _tagManager;
   
        public AdminViewProvider(
            ITagManager<Tag> tagManager)
        {
  
            _tagManager = tagManager;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(TagAdmin label, IViewProviderContext context)
        {

            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(TagIndexViewModel<Tag>)] as TagIndexViewModel<Tag>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(TagIndexViewModel<Tag>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<TagIndexViewModel<Tag>>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<TagIndexViewModel<Tag>>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<TagIndexViewModel<Tag>>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(TagAdmin label, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildEditAsync(TagAdmin label, IViewProviderContext updater)
        {

            EditTagViewModel editLabelViewModel = null;
            if (label.Id == 0)
            {
                editLabelViewModel = new EditTagViewModel()
                {
                    IsNewTag = true
                };
            }
            else
            {
                editLabelViewModel = new EditTagViewModel()
                {
                    Id = label.Id,
                    Name = label.Name,
                    Description = label.Description
                };
            }

            return Task.FromResult(Views(
                View<EditTagViewModel>("Admin.Edit.Header", model => editLabelViewModel).Zone("header").Order(1),
                View<EditTagViewModel>("Admin.Edit.Content", model => editLabelViewModel).Zone("content").Order(1),
                View<EditTagViewModel>("Admin.Edit.Actions", model => editLabelViewModel).Zone("actions").Order(1),
                View<EditTagViewModel>("Admin.Edit.Footer", model => editLabelViewModel).Zone("footer").Order(1)
            ));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(TagAdmin label, IViewProviderContext context)
        {

            var model = new EditTagViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(label, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();

            if (context.Updater.ModelState.IsValid)
            {

                var result = await _tagManager.UpdateAsync(new Tag()
                {
                    Id = label.Id,
                    FeatureId = label.FeatureId,
                    Name = model.Name,
                    Description = model.Description
                });

                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(label, context);


        }

    }

}
