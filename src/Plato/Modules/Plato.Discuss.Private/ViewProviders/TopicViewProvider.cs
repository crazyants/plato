using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Discuss.Models;
using Plato.Discuss.Private.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Private.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        public static string HtmlName = "visibility";

        private readonly IAuthorizationService _authorizationService;
        private readonly IContextFacade _contextFacade;     
        private readonly IEntityStore<Topic> _entityStore;
        private readonly HttpRequest _request;
 
        public TopicViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Topic> entityStore,
            IAuthorizationService authorizationService)
        {
            _contextFacade = contextFacade;       
            _entityStore = entityStore;
            _authorizationService = authorizationService;
            _request = httpContextAccessor.HttpContext.Request;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Topic entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Topic entity, IViewProviderContext updater)
        {

            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext context)
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
                    if (key.StartsWith(TopicViewProvider.HtmlName))
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
                            Description = "This topic will be visible to everyone. Chose this option if your sharing public information and don't mind public replies",
                            Value = "public",
                            Checked = selectedValue == "public" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.DiscussPrivateCreatePublic
                                : Permissions.DiscussPrivateToPublic
                        },
                        new SelectDropDownItem()
                        {
                            Text = "Private",
                            Description = "This topic will only be visible to you and our team. Choose this option if your sharing private information.",
                            Value = "private",
                            Checked = selectedValue == "private" ? true : false,
                            Permission = entity.Id == 0
                                ? Permissions.DiscussPrivateCreatePrivate
                                : Permissions.DiscussPrivateToPrivate
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

        public override async Task ComposeTypeAsync(Topic question, IUpdateModel updater)
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
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic model, IViewProviderContext context)
        {

            // Ensure entity exists before attempting to update
            var entity = await _entityStore.GetByIdAsync(model.Id);
            if (entity == null)
            {
                return await BuildEditAsync(model, context);
            }
            
            // Validate model
            if (await ValidateModelAsync(model, context.Updater))
            {
                if (!model.IsNew)
                {
                    model.IsPrivate = GetIsPrivate();
                    await _entityStore.UpdateAsync(model);
                }
          
            }

            return await BuildEditAsync(model, context);

        }
        
        bool GetIsPrivate()
        {

            // Get the follow checkbox 
       
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

    }

}
