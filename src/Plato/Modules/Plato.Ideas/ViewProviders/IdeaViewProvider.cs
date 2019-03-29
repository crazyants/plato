using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Ideas.Models;
using Plato.Ideas.Services;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.ViewModels;
using Plato.Entities.Models;
using Plato.Entities.Services;

namespace Plato.Ideas.ViewProviders
{
    public class IdeaViewProvider : BaseViewProvider<Idea>
    {

        private const string EditorHtmlName = "message";
        
        private readonly IEntityStore<Idea> _entityStore;
        private readonly IPostManager<Idea> _articleManager;
        private readonly IEntityViewIncrementer<Idea> _viewIncrementer;

        private readonly HttpRequest _request;
        
        public IdeaViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Idea> entityStore,
            IPostManager<Idea> articleManager,
            IEntityViewIncrementer<Idea> viewIncrementer)
        {
            _entityStore = entityStore;
            _articleManager = articleManager;
            _viewIncrementer = viewIncrementer;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Idea idea, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Idea>)] as EntityIndexViewModel<Idea>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Entity>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Idea>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Idea>>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<EntityIndexViewModel<Idea>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Idea idea, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Idea, IdeaComment>)] as EntityViewModel<Idea, IdeaComment>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityViewModel<Idea, IdeaComment>).ToString()} has not been registered on the HttpContext!");
            }

            // Increment entity views
            await _viewIncrementer
                .Contextulize(context.Controller.HttpContext)
                .IncrementAsync(idea);

            return Views(
                View<Idea>("Home.Display.Header", model => idea).Zone("header"),
                View<Idea>("Home.Display.Tools", model => idea).Zone("tools"),
                View<Idea>("Home.Display.Sidebar", model => idea).Zone("sidebar"),
                View<EntityViewModel<Idea, IdeaComment>>("Home.Display.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Display.Footer", model => new EditEntityReplyViewModel()
                {
                    EntityId = idea.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer"),
                View<EntityViewModel<Idea, IdeaComment>>("Home.Display.Actions", model => viewModel)
                    .Zone("actions")
                    .Order(int.MaxValue)

            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Idea idea, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = idea.Message;
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
                Id = idea.Id,
                Title = idea.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = idea.Alias
            };
     
            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Idea idea, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditEntityViewModel
            {
                Title = idea.Title,
                Message = idea.Message
            });
        }

        public override async Task ComposeTypeAsync(Idea idea, IUpdateModel updater)
        {

            var model = new EditEntityViewModel
            {
                Title = idea.Title,
                Message = idea.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                idea.Title = model.Title;
                idea.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Idea idea, IViewProviderContext context)
        {
            
            if (idea.IsNewIdea)
            {
                return default(IViewProviderResult);
            }

            var entity = await _entityStore.GetByIdAsync(idea.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(idea, context);
            }
            
            // Validate 
            if (await ValidateModelAsync(idea, context.Updater))
            {
                // Update
                var result = await _articleManager.UpdateAsync(idea);

                // Was there a problem updating the entity?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(idea, context);

        }

    }

}
