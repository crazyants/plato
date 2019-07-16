using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Services;

namespace Plato.Articles.StopForumSpam.ViewProviders
{

    public class ArticleViewProvider : BaseViewProvider<Article>
    {
        private readonly ISpamOperatorManager<Article> _spamOperatorManager;
 
        public ArticleViewProvider(
            ISpamOperatorManager<Article> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<bool> ValidateModelAsync(Article entity, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Article, entity);

            // IF any operators failed ensure we display the operator error message
            var valid = true;
            if (results != null)
            {
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                    {
                        if (result.Operation.CustomMessage)
                        {
                            updater.ModelState.AddModelError(string.Empty,
                                !string.IsNullOrEmpty(result.Operation.Message)
                                    ? result.Operation.Message
                                    : $"Sorry but we've identified your details have been used by known spammers.");
                            valid = false;
                        }
                    }
                }
            }

            return valid;

        }

        public override async Task ComposeModelAsync(Article entity, IUpdateModel updater)
        {
            
            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Article, entity);
            if (results != null)
            {
                foreach (var result in results)
                {
                    // If any operator failed flag entity as SPAM
                    if (!result.Succeeded)
                    {
                        entity.IsSpam = true;
                    }
                }
            }
            
        }


        public override async Task<IViewProviderResult> BuildUpdateAsync(Article entity, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(entity, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Article, entity);

            // Return view
            return await BuildIndexAsync(entity, context);

        }
        
    }

}
