using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Models.Shell;
using Plato.Internal.Tasks.Abstractions;

namespace Plato.Discuss.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private const string EditorHtmlName = "message";


        private readonly IShellSettings _shellSettings;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IPostManager<Topic> _topicManager;
        private readonly HttpRequest _request;
        
        public TopicViewProvider(
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Topic> entityStore,
            IPostManager<Topic> topicManager,
            IShellSettings shellSettings)
        {
            _entityStore = entityStore;
            _topicManager = topicManager;
            _shellSettings = shellSettings;
            _request = httpContextAccessor.HttpContext.Request;
        }

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(Topic topic, IViewProviderContext context)
        {

            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Topic>)] as EntityIndexViewModel<Topic>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Topic>).ToString()} has not been registered on the HttpContext!");
            }

            return Task.FromResult(Views(
                View<EntityIndexViewModel<Topic>>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel<Topic>>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<EntityIndexViewModel<Topic>>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Topic topic, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityViewModel<Topic, Reply>)] as EntityViewModel<Topic, Reply>;
            if (viewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Topic>).ToString()} has not been registered on the HttpContext!");
            }

            await IncrementTopicViewCount(topic, context);

            return Views(
                View<Topic>("Home.Display.Header", model => topic).Zone("header"),
                View<Topic>("Home.Display.Tools", model => topic).Zone("tools"),
                View<Topic>("Home.Display.Sidebar", model => topic).Zone("sidebar"),
                View<EntityViewModel<Topic, Reply>>("Home.Display.Content", model => viewModel).Zone("content"),
                View<EditEntityReplyViewModel>("Home.Display.Footer", model => new EditEntityReplyViewModel()
                {
                    EntityId = topic.Id,
                    EditorHtmlName = EditorHtmlName
                }).Zone("footer")
            );

        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Topic topic, IViewProviderContext updater)
        {

            // Ensures we persist the message between post backs
            var message = topic.Message;
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
                Id = topic.Id,
                Title = topic.Title,
                Message = message,
                EditorHtmlName = EditorHtmlName,
                Alias = topic.Alias
            };
     
            return Task.FromResult(Views(
                View<EditEntityViewModel>("Home.Edit.Header", model => viewModel).Zone("header"),
                View<EditEntityViewModel>("Home.Edit.Content", model => viewModel).Zone("content"),
                View<EditEntityViewModel>("Home.Edit.Footer", model => viewModel).Zone("Footer")
            ));

        }
        
        public override async Task<bool> ValidateModelAsync(Topic topic, IUpdateModel updater)
        {
            return await updater.TryUpdateModelAsync(new EditEntityViewModel
            {
                Title = topic.Title,
                Message = topic.Message
            });
        }

        public override async Task ComposeTypeAsync(Topic topic, IUpdateModel updater)
        {

            var model = new EditEntityViewModel
            {
                Title = topic.Title,
                Message = topic.Message
            };

            await updater.TryUpdateModelAsync(model);

            if (updater.ModelState.IsValid)
            {

                topic.Title = model.Title;
                topic.Message = model.Message;
            }

        }
        
        public override async Task<IViewProviderResult> BuildUpdateAsync(Topic topic, IViewProviderContext context)
        {
            
            if (topic.IsNewTopic)
            {
                return default(IViewProviderResult);
            }

            var entity = await _entityStore.GetByIdAsync(topic.Id);
            if (entity == null)
            {
                return await BuildIndexAsync(topic, context);
            }
            
            // Validate 
            if (await ValidateModelAsync(topic, context.Updater))
            {
                // Update
                var result = await _topicManager.UpdateAsync(topic);

                // Was there a problem updating the entity?
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        context.Updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

            }

            return await BuildEditAsync(topic, context);

        }

        #endregion
        
        #region "Private Methods"

        async Task IncrementTopicViewCount(Topic topic, IViewProviderContext context)
        {


            var cookieName = "plato_reads";

            // Transform tracking cookie into int array
            List<int> values = null;
            var cookie = context.Controller.HttpContext.Request.Cookies[cookieName];
            if (!String.IsNullOrEmpty(cookie))
            {
                values = cookie.ToIntArray().ToList();
            }

            if (values != null)
            {
                // Does the entity we are accessing exist in our tracking cookie
                if (values.Contains(topic.Id))
                {
                    return;
                }
            }

            topic.TotalViews = topic.TotalViews + 1;
            topic.DailyViews = topic.TotalViews.ToSafeDevision(DateTimeOffset.Now.DayDifference(topic.CreatedDate));
            var result = await _entityStore.UpdateAsync(topic);

            if (result != null)
            {

                if (values == null)
                {
                    values = new List<int>();
                }

                values.Add(result.Id);

                // Set cookie to prevent further execution
                var tennantPath = "/";
                if (_shellSettings != null)
                {
                    tennantPath += _shellSettings.RequestedUrlPrefix;
                }

                context.Controller.HttpContext.Response.Cookies.Append(
                    cookieName,
                    values.ToArray().ToDelimitedString(),
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Path = tennantPath,
                        Expires = DateTime.Now.AddMinutes(20)
                    });
                
            }
            
        }

        #endregion

    }

}
