using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Docs.Drafts.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Docs.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Docs.Drafts.ViewProviders
{
    public class DocViewProvider : BaseViewProvider<Doc>
    {

        public static string HtmlName = "published";

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly HttpRequest _request;
       
        public DocViewProvider(
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Doc> entityStore)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _authorizationService = authorizationService;
            _entityStore = entityStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Doc article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Doc article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Doc entity, IViewProviderContext context)
        {
            // We always need an entity
            if (entity == null)
            {
                return default(IViewProviderResult);
            }

            // Initial values
            var isHidden = entity.IsHidden;
            var isPrivate = entity.IsPrivate;

            if (entity.Id == 0)
            {
                isHidden = false;
                isPrivate = true;
            }

            // Ensures we persist selection between post backs
            if (context.Controller.HttpContext.Request.Method == "POST")
            {
                foreach (string key in context.Controller.HttpContext.Request.Form.Keys)
                {
                    if (key.StartsWith(DocViewProvider.HtmlName))
                    {
                        var values = context.Controller.HttpContext.Request.Form[key];
                        foreach (var value in values)
                        {
                            if (value.IndexOf("private", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isPrivate = true;
                            }

                            if (value.IndexOf("public", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                isPrivate = false;
                            }
                        }
                    }
                }
            }

            // Build dropdown view model
            var selectedValue = isHidden ? "hidden" : (isPrivate ? "private" : "public");
            var viewModel = new VisibilityDropDownViewModel()
            {
                HtmlName = HtmlName,
                SelectedValue = selectedValue,
                DropDown = new SelectDropDown()
                {
                    Title = "Visibility",
                    InnerCssClass = "d-block",
                    Items = new List<SelectDropDownItem>()
                    {
                        new SelectDropDownItem()
                        {
                            Text = "Private",
                            Description = "The doc will only be visible to you and those with permission to view private docs. Choose this option whilst your authoring your doc.",
                            Value = "private",
                            Checked = selectedValue == "private" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.DocsDraftCreatePrivate
                                : Permissions.DocsDraftToPrivate
                        },
                        new SelectDropDownItem()
                        {
                            Text = "Hidden",
                            Description =
                                "The doc will only be visible to those with permission to view hidden docs. Choose this option once your doc is ready for review.",
                            Value = "hidden",
                            Checked = selectedValue == "hidden" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.DocsDraftCreateHidden
                                : Permissions.DocsDraftToHidden
                        },
                        new SelectDropDownItem()
                        {
                            Text = "Public",
                            Description =
                                "The doc will be visible to everyone. Chose this option once your ready to publish to the world.",
                            Value = "public",
                            Checked = selectedValue == "public" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.DocsDraftCreatePublic
                                : Permissions.DocsDraftToPublic
                        }

                    }
                }
            };

            // For new entities adjust model to ensure the first appropriate
            // option is selected based on our current permissions 
            if (entity.Id == 0)
            {
                await viewModel.AdjustInitiallySelected(_authorizationService, context.Controller.User);
            }

            // Add  dropdown view model to context for use within navigation provider
            context.Controller.HttpContext.Items[typeof(VisibilityDropDownViewModel)] = viewModel;

            // No view modifications 
            return default(IViewProviderResult);


        }

        public override async Task ComposeModelAsync(Doc question, IUpdateModel updater)
        {

            var model = new SelectDropDownViewModel()
            {
                SelectedValue = GetIsPrivate().ToString()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                question.IsPrivate = GetIsPrivate();
                question.IsHidden = GetIsHidden();
            }

        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(Doc model, IViewProviderContext context)
        {

            //// Ensure entity exists before attempting to update
            //var entity = await _entityStore.GetByIdAsync(model.Id);
            //if (entity == null)
            //{
            //    return await BuildEditAsync(model, context);
            //}

            //// Validate model
            //if (await ValidateModelAsync(model, context.Updater))
            //{
            //    if (!model.IsNew)
            //    {
            //        model.IsPrivate = GetIsPrivate();
            //        model.IsHidden = GetIsHidden();
            //        //await _entityStore.UpdateAsync(model);
            //    }

            //}

            return await BuildEditAsync(model, context);


        }

        bool GetIsPrivate()
        {
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (value.IndexOf("private", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        bool GetIsHidden()
        {
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (value.IndexOf("hidden", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

    }

}
