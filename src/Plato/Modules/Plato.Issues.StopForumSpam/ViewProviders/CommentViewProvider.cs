using System.Threading.Tasks;
using Plato.Ideas.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Services;

namespace Plato.Ideas.StopForumSpam.ViewProviders
{

    public class CommentViewProvider : BaseViewProvider<IdeaComment>
    {
        
        private readonly ISpamOperatorManager<IdeaComment> _spamOperatorManager;
    
        public CommentViewProvider(ISpamOperatorManager<IdeaComment> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(IdeaComment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(IdeaComment model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(IdeaComment reply, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(IdeaComment reply, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Comment, reply);

            // If any operators failed and it has a error message
            // ensure we display the operator error message
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

        public override async Task ComposeModelAsync(IdeaComment reply, IUpdateModel updater)
        {

            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Comment, reply);
            if (results != null)
            {
                foreach (var result in results)
                {
                    // If any operator failed flag reply as SPAM
                    if (!result.Succeeded)
                    {
                        reply.IsSpam = true;
                    }
                }
            }

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(IdeaComment reply, IViewProviderContext context)
        {
            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Comment, reply);
            
            // Return view
            return await BuildEditAsync(reply, context);

        }
        
    }

}
