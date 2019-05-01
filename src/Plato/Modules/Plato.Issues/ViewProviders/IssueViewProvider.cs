using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Issues.Models;
using Plato.Issues.Services;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;

namespace Plato.Issues.ViewProviders
{
    public class IssueViewProvider : BaseViewProvider<Issue>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityStore<Issue> _entityStore;
        private readonly IPostManager<Issue> _articleManager;
        private readonly IEntityViewIncrementer<Issue> _viewIncrementer;

        private readonly HttpRequest _request;
        
        public IssueViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Issue> entityStore,
            IPostManager<Issue> articleManager,
            IEntityViewIncrementer<Issue> viewIncrementer)
        {
            _entityStore = entityStore;
            _articleManager = articleManager;
            _viewIncrementer = viewIncrementer;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Issue issue, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Issue>)] as EntityIndexViewModel<Issue>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Issue>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Issue>>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<EntityIndexViewModel<Issue>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Issue issue, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Issue, Comment>)] as EntityViewModel<Issue, Comment>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityViewModel<Issue, Comment>).ToString()} has not been registered on the HttpContext!");
            }

            // Increment entity views
            await _viewIncrementer
                .Contextulize(context.Controller.HttpContext)
                .IncrementAsync(issue);

            return Views(
                View<Issue>("Home.Display.Header", model => issue).Zone("header"),
                View<Issue>("Home.Display.Tools", model => issue).Zone("tools"),
                View<Issue>("Home.Display.Sidebar", model => issue).Zone("sidebar"),
                View<EntityViewModel<Issue, Comment>>("Home.Display.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Display.Footer", model => new EditEntityReplyViewModel()
                {
                    EntityId = issue.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer"),
                View<EntityViewModel<Issue, Comment>>("Home.Display.Actions", model => viewModel)
                    .Zone("actions")
                    .Order(int.MaxValue)
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Issue issue, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = issue.Message;
            if (_request.Method == "POST")
            {
                foreach (string key in _request.Form.Keys)
                {
                    if (key == EditorHtmlName)
                    {
                        message = _request.Form[key];
                    }
                }
            }
          
            var viewModel = new EditEntityViewModel()
            {
                Id = issue.Id,
                Title = issue.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = issue.Alias
            };
     
            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Issue issue, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditEntityViewModel
            {
                Title = issue.Title,
                Message = issue.Message
            });
        }

        public override async Task ComposeTypeAsync(Issue issue, IUpdateModel updater)
        {

            var model = new EditEntityViewModel
            {
                Title = issue.Title,
                Message = issue.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                issue.Title = model.Title;
                issue.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Issue issue, IViewProviderContext context)
        {
            
            if (issue.IsNew)
            {
                return default(IViewProviderResult);
            }

            var entity = await _entityStore.GetByIdAsync(issue.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(issue, context);
            }
            
            // Validate 
            if (await ValidateModelAsync(issue, context.Updater))
            {
                // Update
                var result = await _articleManager.UpdateAsync(issue);

                // Was there a problem updating the entity?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(issue, context);

        }

    }

}
