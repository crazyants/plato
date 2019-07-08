using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Drafts.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Articles.Drafts.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        public static string HtmlName = "published";

        private readonly IAuthorizationService _authorizationService;
        private readonly HttpRequest _request;
        private readonly IEntityStore<Article> _entityStore;

        public ArticleViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Article> entityStore,
            IAuthorizationService authorizationService)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _entityStore = entityStore;
            _authorizationService = authorizationService;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Article article, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext context)
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
                    if (key.StartsWith(ArticleViewProvider.HtmlName))
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
                HtmlName = ArticleViewProvider.HtmlName,
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
                            Description =
                                "The article will only be visible to you and those with permission to view private articles. Choose this option whilst your authoring your article.",
                            Value = "private",
                            Checked = selectedValue == "private" ? true : false,
                            Permission = entity.Id == 0 
                                ? Permissions.ArticlesDraftCreatePrivate
                                : Permissions.ArticlesDraftToPrivate
                        },
                        new SelectDropDownItem()
                        {
                            Text = "Ready for Review",
                            Description =
                                "The article will be visible to those with permission to view hidden articles. Choose this option once your article is ready for review.",
                            Value = "hidden",
                            Checked = selectedValue == "hidden" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.ArticlesDraftCreateHidden
                                : Permissions.ArticlesDraftToHidden
                        },
                        new SelectDropDownItem()
                        {
                            Text = "Public",
                            Description =
                                "The article will be visible to everyone. Chose this option once your ready to publish to the world",
                            Value = "public",
                            Checked = selectedValue == "public" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.ArticlesDraftCreatePublic
                                : Permissions.ArticlesDraftToPublic
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

        public override async Task ComposeTypeAsync(Article question, IUpdateModel updater)
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


        public override async Task<IViewProviderResult> BuildUpdateAsync(Article model, IViewProviderContext context)
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
            //        await _entityStore.UpdateAsync(model);
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
