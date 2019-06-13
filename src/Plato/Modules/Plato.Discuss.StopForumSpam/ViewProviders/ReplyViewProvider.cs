using System.Threading.Tasks;
using Plato.Discuss.Models;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Net.Abstractions;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.StopForumSpam.Services;

namespace Plato.Discuss.StopForumSpam.ViewProviders
{

    public class ReplyViewProvider : BaseViewProvider<Reply>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;
        private readonly ISpamOperatorManager<Reply> _spamOperatorManager;
        private readonly IClientIpAddress _clientIpAddress;

        public ReplyViewProvider(
            IPlatoUserStore<User> platoUserStore, 
            ISpamOperatorManager<Reply> spamOperatorManager,
            IClientIpAddress clientIpAddress)
        {
            _platoUserStore = platoUserStore;
            _spamOperatorManager = spamOperatorManager;
            _clientIpAddress = clientIpAddress;
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

            // Build user to validate
            var user = await BuildUserAsync(reply);

            // Could not build user details from supplied username
            if (user == null)
            {
                return true;
            }

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

        public override async Task<IViewProviderResult> BuildUpdateAsync(Reply reply, IViewProviderContext context)
        {
            if (!context.Updater.ModelState.IsValid)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Build user to validate
            var user = await BuildUserAsync(reply);

            // Could not build user details from supplied username
            if (user == null)
            {
                return await BuildIndexAsync(reply, context);
            }

            // Execute UpdateModel within registered spam operators
            await _spamOperatorManager.UpdateModelAsync(SpamOperations.Reply, reply);


            return await BuildEditAsync(reply, context);

        }

        async Task<User> BuildUserAsync(Reply reply)
        {

            var user = await _platoUserStore.GetByIdAsync(reply.CreatedUserId);
            if (user == null)
            {
                return null;
            }
            
            // Ensure we check against the IP address being used at the time of the post
            user.IpV4Address = _clientIpAddress.GetIpV4Address();
            user.IpV6Address =  _clientIpAddress.GetIpV6Address();
            return user;

        }

    }

}
