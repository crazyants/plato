using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Docs.Models;
using Plato.Docs.Services;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Docs.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Doc>
    {

        public const string EditorHtmlName = "message";
        
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IPostManager<Doc> _topicManager;
        private readonly IEntityViewIncrementer<Doc> _viewIncrementer;

        private readonly HttpRequest _request;
        
        public TopicViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Doc> entityStore,
            IPostManager<Doc> topicManager,
            IEntityViewIncrementer<Doc> viewIncrementer)
        {
            _entityStore = entityStore;
            _topicManager = topicManager;
            _viewIncrementer = viewIncrementer;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Doc doc, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Doc>)] as EntityIndexViewModel<Doc>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Doc>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Doc>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Doc>>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<EntityIndexViewModel<Doc>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Doc doc, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Doc, DocComment>)] as EntityViewModel<Doc, DocComment>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityViewModel<Doc, DocComment>).ToString()} has not been registered on the HttpContext!");
            }

            // Increment entity views
            await _viewIncrementer
                .Contextulize(context.Controller.HttpContext)
                .IncrementAsync(doc);

            return Views(
                View<Doc>("Home.Display.Header", model => doc).Zone("header"),
                View<Doc>("Home.Display.Tools", model => doc).Zone("tools"),
                View<Doc>("Home.Display.Sidebar", model => doc).Zone("sidebar"),
                View<EntityViewModel<Doc, DocComment>>("Home.Display.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Display.Footer", model => new EditEntityReplyViewModel()
                {
                    EntityId = doc.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer").Order(int.MinValue),
                View<EntityViewModel<Doc, DocComment>>("Home.Display.Actions", model => viewModel)
                    .Zone("actions")
                    .Order(int.MaxValue)
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Doc doc, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = doc.Message;
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
                Id = doc.Id,
                Title = doc.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = doc.Alias
            };
     
            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Doc doc, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditEntityViewModel
            {
                Title = doc.Title,
                Message = doc.Message
            });
        }

        public override async Task ComposeTypeAsync(Doc doc, IUpdateModel updater)
        {

            var model = new EditEntityViewModel
            {
                Title = doc.Title,
                Message = doc.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                doc.Title = model.Title;
                doc.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Doc doc, IViewProviderContext context)
        {
            
            if (doc.IsNewTopic)
            {
                return default(IViewProviderResult);
            }

            var entity = await _entityStore.GetByIdAsync(doc.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(doc, context);
            }
            
            // Validate 
            if (await ValidateModelAsync(doc, context.Updater))
            {
                // Update
                var result = await _topicManager.UpdateAsync(doc);

                // Was there a problem updating the entity?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(doc, context);

        }
        
    }

}
