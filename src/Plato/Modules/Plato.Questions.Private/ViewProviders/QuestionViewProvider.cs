using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Core.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Entities.Stores;
using Plato.Internal.Layout.ModelBinding;
using Plato.Questions.Models;
using Plato.Entities.ViewModels;

namespace Plato.Questions.Private.ViewProviders
{
    public class QuestionViewProvider : BaseViewProvider<Question>
    {

        public static string HtmlName = "visibility";

        private readonly IContextFacade _contextFacade;     
        private readonly IEntityStore<Question> _entityStore;
        private readonly HttpRequest _request;
 
        public QuestionViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor,
            IEntityStore<Question> entityStore)
        {
            _contextFacade = contextFacade;       
            _entityStore = entityStore;
        
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

        public override Task<IViewProviderResult> BuildEditAsync(Question entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
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
                if (!question.IsNewQuestion)
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
