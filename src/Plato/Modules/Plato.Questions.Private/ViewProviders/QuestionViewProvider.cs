using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Questions.Models;
using Plato.Internal.Navigation.Abstractions;
using Plato.Questions.Private.ViewModels;

namespace Plato.Questions.Private.ViewProviders
{
    public class QuestionViewProvider : BaseViewProvider<Question>
    {

        public static string HtmlName = "visibility";

        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityStore<Question> _entityStore;
        private readonly HttpRequest _request;
 
        public QuestionViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Question> entityStore,
            IAuthorizationService authorizationService)
        {
            _entityStore = entityStore;
            _authorizationService = authorizationService;
            _request = httpContextAccessor.HttpContext.Request;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Question entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Question entity, IViewProviderContext updater)
        {

            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Question entity, IViewProviderContext context)
        {

            // We always need an entity
            if (entity == null)
            {
                return default(IViewProviderResult);
            }

            // Set isPrivate flag
            var isPrivate = entity.Id > 0 && entity.IsPrivate;

            // Ensures we persist selection between post backs
            if (context.Controller.HttpContext.Request.Method == "POST")
            {
                foreach (string key in context.Controller.HttpContext.Request.Form.Keys)
                {
                    if (key.StartsWith(QuestionViewProvider.HtmlName))
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
            var selectedValue = isPrivate ? "private" : "public";
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
                            Text = "Public",
                            Description = "This question will be visible to everyone. Chose this option if your sharing public information and don't mind public comments",
                            Value = "public",
                            Checked = selectedValue == "public" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.QuestionsPrivateCreatePublic
                                : Permissions.QuestionsPrivateToPublic
                        },
                        new SelectDropDownItem()
                        {
                            Text = "Private",
                            Description = "This question will only be visible to you and our team. Choose this option if your sharing private information.",
                            Value = "private",
                            Checked = selectedValue == "private" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.QuestionsPrivateCreatePrivate
                                : Permissions.QuestionsPrivateToPrivate
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

        public override async Task ComposeTypeAsync(Question question, IUpdateModel updater)
        {

            var model = new SelectDropDownViewModel()
            {
                SelectedValue = GetIsPrivate().ToString()
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {
                question.IsPrivate = GetIsPrivate();
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Question question, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(question.Id);
            if (entity == null)
            {
                return await BuildEditAsync(question, context);
            }
            
            // Validate model
            if (await ValidateModelAsync(question, context.Updater))
            {
                if (!question.IsNew)
                {
                    question.IsPrivate = GetIsPrivate();
                    await _entityStore.UpdateAsync(question);
                }
          
            }

            return await BuildEditAsync(question, context);

        }

        bool GetIsPrivate()
        {

            // Get the follow checkbox value
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (value.Equals("private", StringComparison.OrdinalIgnoreCase))
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
