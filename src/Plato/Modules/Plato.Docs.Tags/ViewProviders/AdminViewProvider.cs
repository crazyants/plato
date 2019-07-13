using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Tags.Services;
using Plato.Tags.ViewModels;
using Plato.Docs.Tags.Models;
using Plato.Docs.Tags.ViewModels;

namespace Plato.Docs.Tags.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<TagAdmin>
    {

        private readonly ITagManager<Tag> _tagManager;
   
        public AdminViewProvider(
            ITagManager<Tag> tagManager)
        {
  
            _tagManager = tagManager;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(TagAdmin tag, IViewProviderContext context)
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

        public override Task<IViewProviderResult> BuildDisplayAsync(TagAdmin tag, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildEditAsync(TagAdmin tag, IViewProviderContext updater)
        {

            EditTagViewModel editLabelViewModel = null;
            if (tag.Id == 0)
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
                    Id = tag.Id,
                    Name = tag.Name,
                    Description = tag.Description
                };
            }

            return Task.FromResult(Views(
                View<EditTagViewModel>("Admin.Edit.Header", model => editLabelViewModel).Zone("header").Order(1),
                View<EditTagViewModel>("Admin.Edit.Content", model => editLabelViewModel).Zone("content").Order(1),
                View<EditTagViewModel>("Admin.Edit.Actions", model => editLabelViewModel).Zone("actions").Order(1),
                View<EditTagViewModel>("Admin.Edit.Footer", model => editLabelViewModel).Zone("footer").Order(1)
            ));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(TagAdmin tag, IViewProviderContext context)
        {

            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (tag.IsNewTag)
            {
                return await BuildEditAsync(tag, context);
            }


            var model = new EditTagViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(tag, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();

            if (context.Updater.ModelState.IsValid)
            {

                // Update tag
                tag.Name = model.Name;
                tag.Description = model.Description;
                
                var result = await _tagManager.UpdateAsync((Tag) tag);

                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(tag, context);
            
        }

    }

}
