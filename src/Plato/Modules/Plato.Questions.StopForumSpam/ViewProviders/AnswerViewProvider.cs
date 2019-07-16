using System.Threading.Tasks;
using Plato.Questions.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.StopForumSpam.Services;

namespace Plato.Questions.StopForumSpam.ViewProviders
{

    public class AnswerViewProvider : BaseViewProvider<Answer>
    {
        
        private readonly ISpamOperatorManager<Answer> _spamOperatorManager;
    
        public AnswerViewProvider(ISpamOperatorManager<Answer> spamOperatorManager)
        {
            _spamOperatorManager = spamOperatorManager;
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Answer model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Answer model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Answer reply, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<bool> ValidateModelAsync(Answer reply, IUpdateModel updater)
        {
            
            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Answer, reply);

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

        public override async Task ComposeModelAsync(Answer reply, IUpdateModel updater)
        {

            if (!updater.ModelState.IsValid)
            {
                return;
            }

            // Validate model within registered spam operators
            var results = await _spamOperatorManager.ValidateModelAsync(SpamOperations.Answer, reply);
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

        public override async Task<IViewProviderResult> BuildUpdateAsync(Answer reply, IViewProviderContext context)
        {
            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Answer, reply);
            
            // Return view
            return await BuildEditAsync(reply, context);

        }
        
    }

}
