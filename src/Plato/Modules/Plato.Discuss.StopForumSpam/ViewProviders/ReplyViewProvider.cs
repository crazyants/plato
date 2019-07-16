using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Services;

namespace Plato.Discuss.StopForumSpam.ViewProviders
{

    public class ReplyViewProvider : BaseViewProvider<Reply>
    {
        
        private readonly ISpamOperatorManager<Reply> _spamOperatorManager;
    
        public ReplyViewProvider(ISpamOperatorManager<Reply> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Reply model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Reply reply, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(Reply reply, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Reply, reply);

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

        public override async Task ComposeModelAsync(Reply reply, IUpdateModel updater)
        {

            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Reply, reply);
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

        public override async Task<IViewProviderResult> BuildUpdateAsync(Reply reply, IViewProviderContext context)
        {
            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Reply, reply);
            
            // Return view
            return await BuildEditAsync(reply, context);

        }
        
    }

}
