using System;
using System.Threading.Tasks;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Services;
using Plato.Labels.ViewModels;
using Plato.Issues.Labels.Models;
using Plato.Issues.Labels.ViewModels;

namespace Plato.Issues.Labels.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<LabelAdmin>
    {
 
        private readonly ILabelManager<Label> _labelManager;
      
        public AdminViewProvider(
            ILabelManager<Label> labelManager)
        {
            _labelManager = labelManager;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(LabelAdmin label, IViewProviderContext context)
        {
     
            // Get index view model from context
            var viewModel = context.Controller.HttpContext.Items[typeof(LabelIndexViewModel<Label>)] as LabelIndexViewModel<Label>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(LabelIndexViewModel<Label>).ToString()} has not been registered on the HttpContext!");
            }
            
            return Task.FromResult(Views(
                View<LabelIndexViewModel<Label>>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<LabelIndexViewModel<Label>>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<LabelIndexViewModel<Label>>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(LabelAdmin label, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(LabelAdmin label, IViewProviderContext updater)
        {

            EditLabelViewModel editLabelViewModel = null;
            if (label.Id == 0)
            {
                editLabelViewModel = new EditLabelViewModel()
                {
                    IsNewLabel = true
                };
            }
            else
            {
                editLabelViewModel = new EditLabelViewModel()
                {
                    Id = label.Id,
                    Name = label.Name,
                    Description = label.Description,
                    ForeColor = label.ForeColor,
                    BackColor = label.BackColor
                };
            }
            
            return Task.FromResult(Views(
                View<EditLabelViewModel>("Admin.Edit.Header", model => editLabelViewModel).Zone("header").Order(1),
                View<EditLabelViewModel>("Admin.Edit.Content", model => editLabelViewModel).Zone("content").Order(1),
                View<EditLabelViewModel>("Admin.Edit.Actions", model => editLabelViewModel).Zone("actions").Order(1),
                View<EditLabelViewModel>("Admin.Edit.Footer", model => editLabelViewModel).Zone("footer").Order(1)
            ));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(LabelAdmin label, IViewProviderContext context)
        {

            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            if (label.IsNewLabel)
            {
                return await BuildEditAsync(label, context);
            }

            var model = new EditLabelViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(label, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
            
            if (context.Updater.ModelState.IsValid)
            {

                label.Name = model.Name;
                label.Description = model.Description;
                label.ForeColor = model.ForeColor;
                label.BackColor = model.BackColor;
                
                var result = await _labelManager.UpdateAsync((Label) label);

                foreach (var error in result.Errors)
                {
                    context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return await BuildEditAsync(label, context);
            
        }
        
    }

}
