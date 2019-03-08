using System;
using System.Threading.Tasks;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Labels.Services;
using Plato.Labels.Stores;
using Plato.Labels.ViewModels;
using Plato.Discuss.Labels.Models;
using Plato.Discuss.Labels.ViewModels;

namespace Plato.Discuss.Labels.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<LabelAdmin>
    {
 
        private readonly ILabelStore<Label> _labelStore;
        private readonly ILabelManager<Label> _labelManager;
        private readonly IFeatureFacade _featureFacade;

        public AdminViewProvider(
            ILabelStore<Label> labelStore,
            ILabelManager<Label> labelManager,
            IFeatureFacade featureFacade)
        {
            _labelStore = labelStore;
            _labelManager = labelManager;
            _featureFacade = featureFacade;
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

            var model = new EditLabelViewModel();

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(label, context);
            }

            model.Name = model.Name?.Trim();
            model.Description = model.Description?.Trim();
            
            if (context.Updater.ModelState.IsValid)
            {
                
                var result = await _labelManager.UpdateAsync(new Label()
                {
                    Id = label.Id,
                    FeatureId = label.FeatureId,
                    Name = model.Name,
                    Description = model.Description,
                    ForeColor = model.ForeColor,
                    BackColor = model.BackColor
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
