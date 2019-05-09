using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;

namespace Plato.Discuss.Private.ViewProviders
{
    public class TopicViewProvider : BaseViewProvider<Topic>
    {

        private readonly IContextFacade _contextFacade;     
        private readonly IEntityStore<Topic> _entityStore;
        private readonly HttpRequest _request;
 
        public TopicViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Topic> entityStore)
        {
            _contextFacade = contextFacade;       
            _entityStore = entityStore;
        
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

        public override Task<IViewProviderResult> BuildEditAsync(Topic entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task ComposeTypeAsync(Topic question, IUpdateModel updater)
        {

            var model = new EntityIsPrivateDropDownViewModel
            {
                IsPrivate = GetIsPrivate()
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
                if (!model.IsNewTopic)
                {
                    model.IsPrivate = GetIsPrivate();
                    await _entityStore.UpdateAsync(model);
                }
          
            }

            return await BuildEditAsync(model, context);

        }

        bool GetIsPrivate()
        {

            // Get the follow checkbox value
            var htmlName = new EntityIsPrivateDropDownViewModel().HtmlName;
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(htmlName))
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
