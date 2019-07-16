using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Services;

namespace Plato.Ideas.StopForumSpam.ViewProviders
{

    public class IdeaViewProvider : BaseViewProvider<Idea>
    {
        private readonly ISpamOperatorManager<Idea> _spamOperatorManager;
 
        public IdeaViewProvider(
            ISpamOperatorManager<Idea> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Idea entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Idea entity, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildEditAsync(Idea entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<bool> ValidateModelAsync(Idea entity, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Idea, entity);

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

        public override async Task ComposeModelAsync(Idea entity, IUpdateModel updater)
        {
            
            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Idea, entity);
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


        public override async Task<IViewProviderResult> BuildUpdateAsync(Idea entity, IViewProviderContext context)
        {

            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(entity, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Idea, entity);

            // Return view
            return await BuildIndexAsync(entity, context);

        }
        
    }

}
