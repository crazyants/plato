using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Services;

namespace Plato.Issues.StopForumSpam.ViewProviders
{

    public class IdeaViewProvider : BaseViewProvider<Issue>
    {
        private readonly ISpamOperatorManager<Issue> _spamOperatorManager;
 
        public IdeaViewProvider(
            ISpamOperatorManager<Issue> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Issue entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Issue entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Issue entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<bool> ValidateModelAsync(Issue entity, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Issue, entity);

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

        public override async Task ComposeModelAsync(Issue entity, IUpdateModel updater)
        {
            
            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Issue, entity);
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


        public override async Task<IViewProviderResult> BuildUpdateAsync(Issue entity, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(entity, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Issue, entity);

            // Return view
            return await BuildIndexAsync(entity, context);

        }
        
    }

}
