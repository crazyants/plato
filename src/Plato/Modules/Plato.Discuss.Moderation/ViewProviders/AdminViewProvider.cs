using System;
using System.Threading.Tasks;
using Plato.Discuss.Moderation.Models;
using Plato.Discuss.Moderation.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Models.Shell;
using Plato.Moderation.Models;


namespace Plato.Discuss.Moderation.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<Moderator>
    {

        private readonly IContextFacade _contextFacade;
   

        public AdminViewProvider(
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        #region "Implementation"

        public override async Task<IViewProviderResult> BuildIndexAsync(Moderator oldModerator, IUpdateModel updater)
        {
            var viewModel = await GetIndexModel();

            return Views(
                View<ModeratorIndexViewModel>("Admin.Index.Header", model => viewModel).Zone("header").Order(1),
                View<ModeratorIndexViewModel>("Admin.Index.Tools", model => viewModel).Zone("tools").Order(1),
                View<ModeratorIndexViewModel>("Admin.Index.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Moderator oldModerator, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));

        }

        public override Task<IViewProviderResult> BuildEditAsync(Moderator oldModerator, IUpdateModel updater)
        {

            var viewModel = new EditModeratorViewModel();
            viewModel.IsNewModerator = oldModerator.Id == 0;
            
            return Task.FromResult(Views(
                View<EditModeratorViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<EditModeratorViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1),
                View<EditModeratorViewModel>("Admin.Edit.Actions", model => viewModel).Zone("actions").Order(1),
                View<EditModeratorViewModel>("Admin.Edit.Footer", model => viewModel).Zone("footer").Order(1)
            ));
        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Moderator oldModerator, IUpdateModel updater)
        {

         
            //var model = new EditLabelViewModel();

            //if (!await updater.TryUpdateModelAsync(model))
            //{
            //    return await BuildEditAsync(label, updater);
            //}

            //model.Name = model.Name?.Trim();
            //model.Description = model.Description?.Trim();

            ////Category category = null;

            //if (updater.ModelState.IsValid)
            //{

            //    var iconCss = model.IconCss;
            //    if (!string.IsNullOrEmpty(iconCss))
            //    {
            //        iconCss = model.IconPrefix + iconCss;
            //    }

            //    var result = await _labelManager.UpdateAsync(new Label()
            //    {
            //        Id = label.Id,
            //        FeatureId = label.FeatureId,
            //        Name = model.Name,
            //        Description = model.Description,
            //        ForeColor = model.ForeColor,
            //        BackColor = model.BackColor,
            //        IconCss = iconCss
            //    });

            //    foreach (var error in result.Errors)
            //    {
            //        updater.ModelState.AddModelError(string.Empty, error.Description);
            //    }

            //}

            return await BuildEditAsync(oldModerator, updater);


        }

        #endregion

        #region "Private Methods"

        async Task<ModeratorIndexViewModel> GetIndexModel()
        {
            var feature = await GetcurrentFeature();
            
            return new ModeratorIndexViewModel()
            {
             
            };
        }

        async Task<ShellModule> GetcurrentFeature()
        {
            var featureId = "Plato.Discuss.Labels";
            var feature = await _contextFacade.GetFeatureByModuleIdAsync(featureId);
            if (feature == null)
            {
                throw new Exception($"No feature could be found for the module '{featureId}'");
            }
            return feature;
        }

        #endregion

    }
}
