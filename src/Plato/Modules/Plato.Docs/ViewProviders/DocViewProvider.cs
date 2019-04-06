using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Docs.Models;
using Plato.Docs.Services;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Docs.ViewProviders
{
    public class DocViewProvider : BaseViewProvider<Doc>
    {

        public const string EditorHtmlName = "message";
        public const string ParentHtmlName = "parent";

        private readonly IEntityViewIncrementer<Doc> _viewIncrementer;
        private readonly IEntityStore<Doc> _entityStore;
        private readonly IPostManager<Doc> _topicManager;
        private readonly IFeatureFacade _featureFacade;

        private readonly HttpRequest _request;
        
        public DocViewProvider(
            IEntityViewIncrementer<Doc> viewIncrementer,
            IHttpContextAccessor httpContextAccessor,
            IPostManager<Doc> topicManager,
            IEntityStore<Doc> entityStore,
            IFeatureFacade featureFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _viewIncrementer = viewIncrementer;
            _featureFacade = featureFacade;
            _topicManager = topicManager;
            _entityStore = entityStore;
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
                View<EntityIndexViewModel<Doc>>("Home.Index.Content", model => viewModel).Zone("content"),
                View<EntityIndexViewModel<Doc>>("Home.Index.Sidebar", model => viewModel).Zone("sidebar")
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
        
        public override async Task<IViewProviderResult> BuildEditAsync(Doc doc, IViewProviderContext updater)
        {

            // Get feature
            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
            if (feature == null)
            {
                throw new Exception("The feature Plato.Docs could not be found");
            }

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
          
            // Build general model
            var viewModel = new EditEntityViewModel()
            {
                Id = doc.Id,
                Title = doc.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = doc.Alias
            };
            
            // Build drop down model
            var entityDropDownViewModel = new EntityDropDownViewModel()
            {
                Options = new EntityIndexOptions()
                {
                    FeatureId = feature.Id
                },
                HtmlName = ParentHtmlName,
                SelectedEntity = doc?.ParentId ?? 0
            };
            
            // Build view
            return Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EntityDropDownViewModel>("Home.Edit.Sidebar", model => entityDropDownViewModel).Zone("sidebar"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            );

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
                doc.ParentId = GetParentSelection();;
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

                doc.ParentId = GetParentSelection();

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
        
        int GetParentSelection()
        {
           
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(ParentHtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        var ok = int.TryParse(value, out var id);
                        if (ok)
                        {
                            return id;
                        }
                    }
                }
            }

            return 0;
        }
        
     
    }

}
